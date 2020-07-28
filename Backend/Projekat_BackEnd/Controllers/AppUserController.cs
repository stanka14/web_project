using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Newtonsoft.Json;
using Projekat_BackEnd.Models;
using Projekat_BackEnd.Models.AppSettings;
using Projekat_BackEnd.Models.ContextData;
using Projekat_BackEnd.Models.Model;

namespace Projekat_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserController : Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private readonly ApplicationSettings _appSettings;
        private ApplicationUsersContext _context2;
        public IConfiguration Configuration { get; }

        public AppUserController(ApplicationUsersContext app2, UserManager<User> userManager, SignInManager<User> signInManager, IOptions<ApplicationSettings> appSettings, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
            _context2 = app2;
            Configuration = configuration;
        }
        [HttpPost]
        [Route("CheckUsername")]
        public async Task<bool> CheckUsername(UsernameModel id)
        {
            bool ret = true;
            var user = await _userManager.FindByNameAsync(id.username);
            if (user == null)
                return true;
            else
                return false;
            //List<User> users = _context2.ApplicationUsers.ToList();
            //foreach(var user in users)
            //{
            //    if (user.UserName == id.username)
            //        ret = false;
            //}
            //return ret;
        }
        [HttpPost]
        [Route("CheckPassword")]
        public async Task<IActionResult> CheckPassword(UsernameModel id)
        {
           
            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userid);
            var ret = await _userManager.CheckPasswordAsync(user, id.username);
            return Ok(new { ret });
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("IsAdmin")]
        public async Task<Object> IsAdmin(IsAdminModel model)
        {
            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            string role = User.Claims.First(c => c.Type == "Roles").Value;
            List<Airline> companies = _context2.AirlineCompanies.ToList();
            int idc = 0;
            foreach(var c in companies)
            {
                if (c.Name == model.id)
                    idc = c.Id;
            }
            var user = await _userManager.FindByIdAsync(userid);

            if (role == "AirlineAdministrator" && user.CompanyId == idc)
                return true;
           
            return false;

        }
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<Object> ChangePassword(ChangePasswordModel model)
        {
            List<User> users = _context2.ApplicationUsers.
               Include(comp => comp.FriendRequests).
                Include(comp => comp.FlightRequests).
               Include(comp => comp.SentRequests)
               .Include(comp => comp.Friends)
               .Include(comp => comp.Notifications)
                .Include(comp => comp.Flights).ThenInclude(f => f.Flight).ThenInclude(f => f.Luggage)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.From)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.To)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Stops)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Seats).ThenInclude(d => d.Traveller)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                .ToList();
            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userid);
            await _userManager.ChangePasswordAsync(user, model.password, model.newPassword);
            user.ChangedPassword = true;
            _context2.Update(user);
            _context2.SaveChanges();
            UserModel um = new UserModel();
            um.FullName = user.FullName;
            um.Address = user.Address;
            um.Birthday = user.Birthday.ToString();
            um.Passport = user.Passport;
            um.OldFlights = new List<TicketListingInfoModel>();
            um.Flights = new List<TicketListingInfoModel>();
            if (user.Flights == null)
                user.Flights = new List<Ticket>();
            foreach (var of in user.Flights)
            {
                if (DateTime.Parse(of.Flight.DepartureDate) < DateTime.Now)
                {
                    um.OldFlights.Add(new TicketListingInfoModel()
                    {
                        seat = of.Seat,
                        discount = of.Discount,
                        id = of.Id
,
                        flight = new FlightTicket()
                        {
                            departureDate = of.Flight.DepartureDate,
                            duration = of.Flight.Duration,
                            id = of.Flight.Id,
                            trip = of.Flight.Trip,
                            prise = of.Flight.Price,
                            from = new DestinationListingInfoModel()
                            {
                                id = of.Flight.From.Id,
                                description = of.Flight.From.Description,
                                img = of.Flight.From.Img,
                                name = of.Flight.From.Name
                            },
                            to = new DestinationListingInfoModel()
                            {
                                id = of.Flight.To.Id,
                                description = of.Flight.To.Description,
                                img = of.Flight.To.Img,
                                name = of.Flight.To.Name
                            }
    ,
                            idCompany = of.Flight.IdCompany
                        }
                    });
                }
                else
                {
                    um.Flights.Add(new TicketListingInfoModel()
                    {
                        seat = of.Seat,
                        discount = of.Discount,
                        id = of.Id
,
                        flight = new FlightTicket()
                        {
                            departureDate = of.Flight.DepartureDate,
                            duration = of.Flight.Duration,
                            id = of.Flight.Id,
                            trip = of.Flight.Trip,
                            prise = of.Flight.Price,
                            from = new DestinationListingInfoModel()
                            {
                                id = of.Flight.From.Id,
                                description = of.Flight.From.Description,
                                img = of.Flight.From.Img,
                                name = of.Flight.From.Name
                            },
                            to = new DestinationListingInfoModel()
                            {
                                id = of.Flight.To.Id,
                                description = of.Flight.To.Description,
                                img = of.Flight.To.Img,
                                name = of.Flight.To.Name
                            }
    ,
                            idCompany = of.Flight.IdCompany
                        }
                    });
                }


            }

            um.Id = user.Id;
            um.Username = user.UserName;
            um.Points = user.Points;
            um.Email = user.Email;
            um.CompanyId = user.CompanyId;

            um.FlightRequests = new List<TicketListingInfoModel>();
            um.Friends = new List<UserModel>();
            um.SentRequests = new List<FriendRequestSentModel>();
            um.Notifications = new List<Notification>();
            um.FriendRequests = new List<FriendRequestReceivedModel>();

            if (user.FlightRequests == null)
                user.FlightRequests = new List<TicketInvitation>();
            foreach (var fr in user.FlightRequests)
            {
                um.FlightRequests.Add(new TicketListingInfoModel()
                {
                    seat = fr.Ticket.Seat,
                    discount = fr.Ticket.Discount,
                    id = fr.Ticket.Id,
                    flight = new FlightTicket()
                    {
                        departureDate = fr.Ticket.Flight.DepartureDate,
                        duration = fr.Ticket.Flight.Duration,
                        from = new DestinationListingInfoModel()
                        {
                            name = fr.Ticket.Flight.From.Name,
                            description = fr.Ticket.Flight.From.Description,
                            id = fr.Ticket.Flight.From.Id,
                            img = fr.Ticket.Flight.From.Img,
                        },
                        to = new DestinationListingInfoModel()
                        {
                            name = fr.Ticket.Flight.To.Name,
                            description = fr.Ticket.Flight.To.Description,
                            id = fr.Ticket.Flight.To.Id,
                            img = fr.Ticket.Flight.To.Img,
                        },
                        id = fr.Ticket.Flight.Id,
                        idCompany = fr.Ticket.Flight.IdCompany,
                        prise = fr.Ticket.Flight.Price,
                        trip = fr.Ticket.Flight.Trip
                    }

                });
            }
            if (user.Friends == null)
                user.Friends = new List<User>();
            foreach (var fr in user.Friends)
            {
                um.Friends.Add(new UserModel()
                {
                    Address = fr.Address,
                    Email = fr.Email,
                    Birthday = fr.Birthday.ToString(),
                    FullName = fr.FullName,
                    Username = fr.UserName,
                    Id = fr.Id,
                    Passport = fr.Passport
                });
            }
            if (user.SentRequests == null)
                user.SentRequests = new List<FriendRequestSent>();
            foreach (var fr in user.SentRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.SentRequests.Add(new FriendRequestSentModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.FriendRequests == null)
                user.FriendRequests = new List<FriendRequestReceived>();
            foreach (var fr in user.FriendRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.FriendRequests.Add(new FriendRequestReceivedModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.Notifications == null)
                user.Notifications = new List<Notification>();
            foreach (var fr in user.Notifications)
            {
                um.Notifications.Add(new Notification() { Id = fr.Id, Text = fr.Text });
            }

            return um;
        }

        [HttpPost]
        [Route("SaveNewAccountDetails")]
        public async Task<Object> SaveNewAccountDetails(ChangeAccountModel model)
        {
            List<User> users = _context2.ApplicationUsers.
               Include(comp => comp.FriendRequests).
                Include(comp => comp.FlightRequests).
               Include(comp => comp.SentRequests)
               .Include(comp => comp.Friends)
               .Include(comp => comp.Notifications)
                .Include(comp => comp.Flights).ThenInclude(f => f.Flight).ThenInclude(f => f.Luggage)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.From)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.To)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Stops)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Seats).ThenInclude(d => d.Traveller)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                .ToList();
            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userid);
            if (user.Email != model.email)
            {
                user.EmailConfirmed = false;
                string toMail = "http://localhost:57886/api/AppUser/VerifyEmail/" + user.Id;

                MailMessage mail = new MailMessage();
                mail.To.Add(model.email);
                mail.From = new MailAddress("web2020tim1718@gmail.com");
                mail.Subject = "Projekat";
                mail.Body = "Please verify your e-mail address by clicking this link: ";
                mail.Body += toMail;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new System.Net.NetworkCredential("web2020tim1718@gmail.com", "NinaStanka987");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
            user.Email = model.email;
            user.FullName = model.fullName;
            user.Address = model.address;
            _context2.Update(user);
            _context2.SaveChanges();
            UserModel um = new UserModel();
            um.FullName = user.FullName;
            um.Address = user.Address;
            um.Birthday = user.Birthday.ToString();
            um.Passport = user.Passport;
            um.OldFlights = new List<TicketListingInfoModel>();
            um.Flights = new List<TicketListingInfoModel>();
            if (user.Flights == null)
                user.Flights = new List<Ticket>();
            foreach (var of in user.Flights)
            {

                if (DateTime.Parse(of.Flight.DepartureDate) < DateTime.Now)
                {
                    um.OldFlights.Add(new TicketListingInfoModel()
                    {
                        seat = of.Seat,
                        discount = of.Discount,
                        id = of.Id
,
                        flight = new FlightTicket()
                        {
                            departureDate = of.Flight.DepartureDate,
                            duration = of.Flight.Duration,
                            id = of.Flight.Id,
                            trip = of.Flight.Trip,
                            prise = of.Flight.Price,
                            from = new DestinationListingInfoModel()
                            {
                                id = of.Flight.From.Id,
                                description = of.Flight.From.Description,
                                img = of.Flight.From.Img,
                                name = of.Flight.From.Name
                            },
                            to = new DestinationListingInfoModel()
                            {
                                id = of.Flight.To.Id,
                                description = of.Flight.To.Description,
                                img = of.Flight.To.Img,
                                name = of.Flight.To.Name
                            }
    ,
                            idCompany = of.Flight.IdCompany
                        }
                    });
                }
                else
                {
                    um.Flights.Add(new TicketListingInfoModel()
                    {
                        seat = of.Seat,
                        discount = of.Discount,
                        id = of.Id
,
                        flight = new FlightTicket()
                        {
                            departureDate = of.Flight.DepartureDate,
                            duration = of.Flight.Duration,
                            id = of.Flight.Id,
                            trip = of.Flight.Trip,
                            prise = of.Flight.Price,
                            from = new DestinationListingInfoModel()
                            {
                                id = of.Flight.From.Id,
                                description = of.Flight.From.Description,
                                img = of.Flight.From.Img,
                                name = of.Flight.From.Name
                            },
                            to = new DestinationListingInfoModel()
                            {
                                id = of.Flight.To.Id,
                                description = of.Flight.To.Description,
                                img = of.Flight.To.Img,
                                name = of.Flight.To.Name
                            }
    ,
                            idCompany = of.Flight.IdCompany
                        }
                    });
                }


            }

            um.Id = user.Id;
            um.Username = user.UserName;
            um.Points = user.Points;
            um.Email = user.Email;
            um.CompanyId = user.CompanyId;

            um.FlightRequests = new List<TicketListingInfoModel>();
            um.Friends = new List<UserModel>();
            um.SentRequests = new List<FriendRequestSentModel>();
            um.Notifications = new List<Notification>();
            um.FriendRequests = new List<FriendRequestReceivedModel>();

            if (user.FlightRequests == null)
                user.FlightRequests = new List<TicketInvitation>();
            foreach (var fr in user.FlightRequests)
            {
                um.FlightRequests.Add(new TicketListingInfoModel()
                {
                    seat = fr.Ticket.Seat,
                    discount = fr.Ticket.Discount,
                    id = fr.Ticket.Id,
                    flight = new FlightTicket()
                    {
                        departureDate = fr.Ticket.Flight.DepartureDate,
                        duration = fr.Ticket.Flight.Duration,
                        from = new DestinationListingInfoModel()
                        {
                            name = fr.Ticket.Flight.From.Name,
                            description = fr.Ticket.Flight.From.Description,
                            id = fr.Ticket.Flight.From.Id,
                            img = fr.Ticket.Flight.From.Img,
                        },
                        to = new DestinationListingInfoModel()
                        {
                            name = fr.Ticket.Flight.To.Name,
                            description = fr.Ticket.Flight.To.Description,
                            id = fr.Ticket.Flight.To.Id,
                            img = fr.Ticket.Flight.To.Img,
                        },
                        id = fr.Ticket.Flight.Id,
                        idCompany = fr.Ticket.Flight.IdCompany,
                        prise = fr.Ticket.Flight.Price,
                        trip = fr.Ticket.Flight.Trip
                    }

                });
            }
            if (user.Friends == null)
                user.Friends = new List<User>();
            foreach (var fr in user.Friends)
            {
                um.Friends.Add(new UserModel()
                {
                    Address = fr.Address,
                    Email = fr.Email,
                    Birthday = fr.Birthday.ToString(),
                    FullName = fr.FullName,
                    Username = fr.UserName,
                    Id = fr.Id,
                    Passport = fr.Passport
                });
            }
            if (user.SentRequests == null)
                user.SentRequests = new List<FriendRequestSent>();
            foreach (var fr in user.SentRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.SentRequests.Add(new FriendRequestSentModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.FriendRequests == null)
                user.FriendRequests = new List<FriendRequestReceived>();
            foreach (var fr in user.FriendRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.FriendRequests.Add(new FriendRequestReceivedModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.Notifications == null)
                user.Notifications = new List<Notification>();
            foreach (var fr in user.Notifications)
            {
                um.Notifications.Add(new Notification() { Id = fr.Id, Text = fr.Text });
            }

            return um;
        }
        [HttpPost]
        [Route("ChangeUserName")]
        public async Task<Object> ChangeUserName(UsernameModel id)
        {
            List<User> users = _context2.ApplicationUsers.
               Include(comp => comp.FriendRequests).
                Include(comp => comp.FlightRequests).
               Include(comp => comp.SentRequests)
               .Include(comp => comp.Friends)
               .Include(comp => comp.Notifications)
                .Include(comp => comp.Flights).ThenInclude(f => f.Flight).ThenInclude(f => f.Luggage)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.From)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.To)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Stops)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Seats).ThenInclude(d => d.Traveller)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                .ToList();
            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userid);
            user.UserName = id.username;
            _context2.Update(user);
            _context2.SaveChanges();
            UserModel um = new UserModel();
            um.FullName = user.FullName;
            um.Address = user.Address;
            um.Birthday = user.Birthday.ToString();
            um.Passport = user.Passport;
            um.OldFlights = new List<TicketListingInfoModel>();
            um.Flights = new List<TicketListingInfoModel>();
            if (user.Flights == null)
                user.Flights = new List<Ticket>();
            foreach (var of in user.Flights)
            {

                if (DateTime.Parse(of.Flight.DepartureDate) < DateTime.Now)
                {
                    um.OldFlights.Add(new TicketListingInfoModel()
                    {
                        seat = of.Seat,
                        discount = of.Discount,
                        id = of.Id
,
                        flight = new FlightTicket()
                        {
                            departureDate = of.Flight.DepartureDate,
                            duration = of.Flight.Duration,
                            id = of.Flight.Id,
                            trip = of.Flight.Trip,
                            prise = of.Flight.Price,
                            from = new DestinationListingInfoModel()
                            {
                                id = of.Flight.From.Id,
                                description = of.Flight.From.Description,
                                img = of.Flight.From.Img,
                                name = of.Flight.From.Name
                            },
                            to = new DestinationListingInfoModel()
                            {
                                id = of.Flight.To.Id,
                                description = of.Flight.To.Description,
                                img = of.Flight.To.Img,
                                name = of.Flight.To.Name
                            }
    ,
                            idCompany = of.Flight.IdCompany
                        }
                    });
                }
                else
                {
                    um.Flights.Add(new TicketListingInfoModel()
                    {
                        seat = of.Seat,
                        discount = of.Discount,
                        id = of.Id
,
                        flight = new FlightTicket()
                        {
                            departureDate = of.Flight.DepartureDate,
                            duration = of.Flight.Duration,
                            id = of.Flight.Id,
                            trip = of.Flight.Trip,
                            prise = of.Flight.Price,
                            from = new DestinationListingInfoModel()
                            {
                                id = of.Flight.From.Id,
                                description = of.Flight.From.Description,
                                img = of.Flight.From.Img,
                                name = of.Flight.From.Name
                            },
                            to = new DestinationListingInfoModel()
                            {
                                id = of.Flight.To.Id,
                                description = of.Flight.To.Description,
                                img = of.Flight.To.Img,
                                name = of.Flight.To.Name
                            }
    ,
                            idCompany = of.Flight.IdCompany
                        }
                    });
                }


            }

            um.Id = user.Id;
            um.Username = user.UserName;
            um.Points = user.Points;
            um.Email = user.Email;
            um.CompanyId = user.CompanyId;

            um.FlightRequests = new List<TicketListingInfoModel>();
            um.Friends = new List<UserModel>();
            um.SentRequests = new List<FriendRequestSentModel>();
            um.Notifications = new List<Notification>();
            um.FriendRequests = new List<FriendRequestReceivedModel>();

            if (user.FlightRequests == null)
                user.FlightRequests = new List<TicketInvitation>();
            foreach (var fr in user.FlightRequests)
            {
                um.FlightRequests.Add(new TicketListingInfoModel()
                {
                    seat = fr.Ticket.Seat,
                    discount = fr.Ticket.Discount,
                    id = fr.Ticket.Id,
                    flight = new FlightTicket()
                    {
                        departureDate = fr.Ticket.Flight.DepartureDate,
                        duration = fr.Ticket.Flight.Duration,
                        from = new DestinationListingInfoModel()
                        {
                            name = fr.Ticket.Flight.From.Name,
                            description = fr.Ticket.Flight.From.Description,
                            id = fr.Ticket.Flight.From.Id,
                            img = fr.Ticket.Flight.From.Img,
                        },
                        to = new DestinationListingInfoModel()
                        {
                            name = fr.Ticket.Flight.To.Name,
                            description = fr.Ticket.Flight.To.Description,
                            id = fr.Ticket.Flight.To.Id,
                            img = fr.Ticket.Flight.To.Img,
                        },
                        id = fr.Ticket.Flight.Id,
                        idCompany = fr.Ticket.Flight.IdCompany,
                        prise = fr.Ticket.Flight.Price,
                        trip = fr.Ticket.Flight.Trip
                    }

                });
            }
            if (user.Friends == null)
                user.Friends = new List<User>();
            foreach (var fr in user.Friends)
            {
                um.Friends.Add(new UserModel()
                {
                    Address = fr.Address,
                    Email = fr.Email,
                    Birthday = fr.Birthday.ToString(),
                    FullName = fr.FullName,
                    Username = fr.UserName,
                    Id = fr.Id,
                    Passport = fr.Passport
                });
            }
            if (user.SentRequests == null)
                user.SentRequests = new List<FriendRequestSent>();
            foreach (var fr in user.SentRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.SentRequests.Add(new FriendRequestSentModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.FriendRequests == null)
                user.FriendRequests = new List<FriendRequestReceived>();
            foreach (var fr in user.FriendRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.FriendRequests.Add(new FriendRequestReceivedModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.Notifications == null)
                user.Notifications = new List<Notification>();
            foreach (var fr in user.Notifications)
            {
                um.Notifications.Add(new Notification() { Id = fr.Id, Text = fr.Text });
            }

            return um;
        }

        [HttpPost]
        [Route("SocialLogIn")]
        public async Task<IActionResult> SocialLogIn(SocialLogInModel model)
        {
            if (!VerifyToken(model.IdToken))
            {
                return BadRequest(new { message = "Account token could not be verified." });
            }
            var user = await _userManager.FindByNameAsync(model.UserId);
            if (user == null)
            {
                // pravimo novog korisnika
                var applicationUser = new User()
                {
                    UserName = model.UserId,
                    Email = model.EmailAddress,
                    FullName = model.FirstName + " " + model.LastName,
                    Birthday = DateTime.Now,
                    Address = "",
                    PhoneNumber = "",
                    Passport = "",
                    SocialUser = true,
                };

                try
                {
                    IdentityResult result = await _userManager.CreateAsync(applicationUser);

                    if (result.Succeeded)
                    {
                        _userManager.AddToRoleAsync(applicationUser, "RegisteredUser").Wait();
                        user = await _userManager.FindByNameAsync(model.UserId);

                        string toMail = "http://localhost:57886/api/AppUser/VerifyEmail/" + user.Id;

                        MailMessage mail = new MailMessage();
                        mail.To.Add(user.Email);
                        mail.From = new MailAddress("web2020tim1718@gmail.com");
                        mail.Subject = "Projekat";
                        mail.Body = "<h1>Please verify your account by clicking on the link below<h1><br>";
                        mail.Body += toMail;

                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.Credentials = new System.Net.NetworkCredential("web2020tim1718@gmail.com", "NinaStanka987");
                            smtp.EnableSsl = true;
                            smtp.Send(mail);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Count > 0)
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID",user.Id.ToString()),
                        new Claim("Roles", roles[0])
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(60),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new { token });
            }
            else
                return BadRequest(new { message = "User does not have a role." });
        }

        private const string GoogleApiTokenInfoUrl = "https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={0}";

        public bool VerifyToken(string providerToken)
        {
            var httpClient = new HttpClient();
            var requestUri = new Uri(string.Format(GoogleApiTokenInfoUrl, providerToken));

            HttpResponseMessage httpResponseMessage;

            try
            {
                httpResponseMessage = httpClient.GetAsync(requestUri).Result;
            }
            catch (Exception ex)
            {
                return false;
            }

            if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }

            var response = httpResponseMessage.Content.ReadAsStringAsync().Result;
            var googleApiTokenInfo = JsonConvert.DeserializeObject<GoogleApiTokenInfo>(response);

            return true;
        }

        [HttpPost]
        [Route("SocialLogInFb")]
        public async Task<IActionResult> SocialLogInFb(SocialLogInModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserId);
            if (user == null)
            {
                // pravimo novog korisnika
                var applicationUser = new User()
                {
                    UserName = model.UserId,
                    Email = model.EmailAddress,
                    FullName = model.FirstName + " " + model.LastName,
                    Birthday = DateTime.Now,
                    Address = "",
                    PhoneNumber = "",
                    Passport = "",
                    SocialUser = true,
                };

                try
                {
                    IdentityResult result = await _userManager.CreateAsync(applicationUser);

                    if (result.Succeeded)
                    {
                        _userManager.AddToRoleAsync(applicationUser, "RegisteredUser").Wait();
                        user = await _userManager.FindByNameAsync(model.UserId);

                        string toMail = "http://localhost:57886/api/AppUser/VerifyEmail/" + user.Id;

                        MailMessage mail = new MailMessage();
                        mail.To.Add(user.Email);
                        mail.From = new MailAddress("web2020tim1718@gmail.com");
                        mail.Subject = "Projekat";
                        mail.Body = "<h1>Please verify your account by clicking on the link below<h1><br>";
                        mail.Body += toMail;

                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.Credentials = new System.Net.NetworkCredential("web2020tim1718@gmail.com", "NinaStanka987");
                            smtp.EnableSsl = true;
                            smtp.Send(mail);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Count > 0)
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID",user.Id.ToString()),
                        new Claim("Roles", roles[0])
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(60),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new { token });
            }
            else
                return BadRequest(new { message = "User does not have a role." });
        }

        [HttpPost]
        [Route("Login")]
        //POST : /api/AppUser/Login
        public async Task<IActionResult> Login(LogInModel model)
        { 
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                if(user.EmailConfirmed)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Count > 0)
                    {
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new Claim[]
                        {
                        new Claim("UserID",user.Id.ToString()),
                        new Claim("Roles", roles[0])
                        }),
                            Expires = DateTime.UtcNow.AddMinutes(60),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                        };
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                        var token = tokenHandler.WriteToken(securityToken);
                        return Ok(new { token });
                    }
                    else
                        return BadRequest(new { message = "User does not have a role." });
                }
                else
                    return BadRequest(new { message = "Please verify your e-mail first." });
            }
            else
                return BadRequest(new { message = "Username or password is incorrect." });
        }

        [HttpPost]
        [Route("Register")]
        //POST : /api/ApplicationUser/Register
        public async Task<IActionResult> Register(RegisterModel rm)
        {
            var user = await _userManager.FindByNameAsync(rm.username);
            if(user != null)
                return BadRequest(new { message = "Username already exists!." });
            var applicationUser = new User()
            {
                UserName = rm.username,
                Email = rm.email,
                FullName = rm.fullName,
                Birthday = DateTime.Parse(rm.birthday),
                Address = rm.address,
                PhoneNumber = rm.phone,
                Passport = rm.passport,
                SocialUser = false,
            };

            try
            {
                IdentityResult result = await _userManager.CreateAsync(applicationUser, rm.password);

                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(applicationUser, "RegisteredUser").Wait();

                    string toMail = "http://localhost:57886/api/AppUser/VerifyEmail/" + applicationUser.Id;

                    MailMessage mail = new MailMessage();
                    mail.To.Add(applicationUser.Email);
                    mail.From = new MailAddress("web2020tim1718@gmail.com");
                    mail.Subject = "Projekat";
                    mail.Body = "<h1>Please verify your account by clicking on the link below<h1><br>";
                    mail.Body += toMail;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new System.Net.NetworkCredential("web2020tim1718@gmail.com", "NinaStanka987");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }

                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet]
        [Route("VerifyEmail/{id}")]
        public async Task VerifyEmail(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            try
            {
                VerifyService service = new VerifyService(Configuration);
                service.Verify(user);
            }
            catch(Exception e)
            {

            }
            
        }

        [HttpPost]
        [Route("SearchUserByPassport")]
        //POST : /api/ApplicationUser/Register
        public UserModel SearchUserByPassport(SearchUserModel sm)
        {

            List<User> users = _context2.ApplicationUsers.ToList();

            foreach(var u in users)
            {
                if(u.Passport == sm.passport)
                {
                    return new UserModel() {Id = u.Id, Username = u.UserName, Passport = u.Passport, FullName = u.FullName, Email = u.Email };
                }
            }


            return null;
        }
        [HttpPost]
        [Route("FastReserve")]

        public async Task<IActionResult> FastReserve(TicketListingInfoModel model)
        {
            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            string role = User.Claims.First(c => c.Type == "Roles").Value;


            var user = await _userManager.FindByIdAsync(userid);

            if (role != "RegisteredUser")
                return BadRequest(new { message = "Not allowed." });

            var users = _context2.Users.Include(us => us.Discount);

            List<Airline> companies = _context2.AirlineCompanies
               .Include(comp => comp.FastTickets)
               .Include(comp => comp.Destinations)
               .Include(comp => comp.PopDestinaations)
               .Include(comp => comp.Raters)
               .Include(comp => comp.Flights).ThenInclude(f => f.Luggage)
               .Include(fs => fs.Flights).ThenInclude(d => d.From)
               .Include(fs => fs.Flights).ThenInclude(d => d.To)
               .Include(fs => fs.Flights).ThenInclude(d => d.Stops)
               .Include(fs => fs.Flights).ThenInclude(d => d.Raters)
               .Include(fs => fs.Flights).ThenInclude(d => d.SoldTickets)
               .Include(fs => fs.Flights).ThenInclude(d => d.Seats).ThenInclude(d => d.Traveller)
               .Include(fs => fs.Flights).ThenInclude(d => d.SoldTickets)
               .Include(fs => fs.Flights).ThenInclude(d => d.Raters)
               .ToList();


            Discount dis = new Discount();

            foreach (var us in users)
            {
                var roles = await _userManager.GetRolesAsync(us);
                if (roles.Contains("WebsiteAdministrator"))
                {
                    dis = us.Discount;
                }
            }
            var options = new DbContextOptionsBuilder<ApplicationUsersContext>()
               .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            using (var context = new ApplicationUsersContext(options))
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    for (int i = 0; i < companies.Count(); i++)
                    {
                        for (int d = 0; d < companies[i].Flights.Count(); d++)
                        {
                            if (companies[i].Flights[d].Id == model.flight.id)
                            {

                                foreach (var a in companies)
                                {
                                    foreach (var f in a.FastTickets)
                                    {
                                        if (f.Id == model.id)
                                        {
                                            if (user.Points > dis.BronzeTier && user.Points < dis.SilverTier)
                                            {
                                                f.Discount = (Int32)(dis.DiscountPercent) + f.Discount;
                                            }

                                            else if (user.Points > dis.SilverTier && user.Points < dis.GoldTier)
                                                f.Discount = (Int32)(dis.DiscountPercent * 2) + f.Discount;
                                            else
                                                f.Discount = (Int32)(dis.DiscountPercent * 3) + f.Discount;
                                            user.Points += 5;
                                            //companies[i].Flights[d].SoldTickets.Add(f);
                                            if(user.Flights == null)
                                                user.Flights = new List<Ticket>();
                                            user.Flights.Add(f);

                                            context.Update(user);
                                            try
                                            {
                                                await context.SaveChangesAsync();
                                            }
                                            catch (DbUpdateConcurrencyException ex)
                                            {
                                                await dbContextTransaction.RollbackAsync();
                                                return BadRequest(new { message = ex.Message });
                                            }
                                            await dbContextTransaction.CommitAsync();

                                        }
                                    }
                                }


                            }
                        }
                    }

                }
            }
           
            return Ok(new { message = "Success." });
        }
        [HttpPost]
        [Route("AddTraveller")]
        //POST : /api/ApplicationUser/Register
        public async Task<Object> AddTraveller(AddTravellerModel model)
        {
            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            string role = User.Claims.First(c => c.Type == "Roles").Value;


            var user = await _userManager.FindByIdAsync(userid);

            if (role != "RegisteredUser")
                return "Forbitten.";
            SeatListingInfoModel slf = new SeatListingInfoModel();

            List<Airline> companies = _context2.AirlineCompanies.Include(comp => comp.Flights).ThenInclude(d => d.Seats).ToList();

            foreach (var u in companies)
            {
                foreach (var u2 in u.Flights)
                {
                    foreach (var u3 in u2.Seats)
                    {
                        if(model.seatId == u3.Id)
                        {
                            u3.Traveller = new Traveller() { Email = model.traveller.email, FirstName = model.traveller.firstName, IdUser = model.traveller.idUser, LastName = model.traveller.lastName, Passport = model.traveller.passport };
                            u3.Taken = true;
                            _context2.SaveChanges();

                            slf = new SeatListingInfoModel()
                            {
                                id = u3.Id,
                                isSelected = u3.IsSelected,
                                taken = u3.Taken,
                                type = u3.Type,
                                traveller = new TravellerInfo()
                                {
                                    email = u3.Traveller.Email,
                                    firstName = u3.Traveller.FirstName,
                                    id = u3.Traveller.Id,
                                    lastName = u3.Traveller.LastName,
                                    idUser = u3.Traveller.IdUser,
                                    passport = u3.Traveller.Passport
                                }
                            };
                        }
                    }
                }
            }


            return slf;
        }
        [HttpPost]
        [Route("DisableSeat")]
        //POST : /api/ApplicationUser/Register
        public SeatListingInfoModel DisableSeat(DisableSeatModel model)
        {
            SeatListingInfoModel slf = new SeatListingInfoModel();

            List<Airline> companies = _context2.AirlineCompanies.Include(comp => comp.Flights).ThenInclude(d => d.Seats).ToList();

            foreach (var u in companies)
            {
                foreach (var u2 in u.Flights)
                {
                    foreach (var u3 in u2.Seats)
                    {
                        if (model.seatId == u3.Id)
                        {
                            u3.Taken = !u3.Taken;
                            _context2.SaveChanges();                     
                        }
                    }
                }
            }


            return slf;
        }
        [HttpPost]
        [Route("CancelFlightRequest")]
        public async Task<Object> CancelFlightRequest(TicketListingInfoModel tim)
        {
            
            List<TicketListingInfoModel> ret = new List<TicketListingInfoModel>();
            List<Airline> companies = _context2.AirlineCompanies.Include(comp => comp.Flights).ThenInclude(d => d.Seats).ToList();

            foreach (var u in companies)
            {
                foreach (var u2 in u.Flights)
                {
                    foreach (var u3 in u2.Seats)
                    {
                        if (tim.seat.Id == u3.Id)
                        {
                            u3.Taken = false;
                            _context2.SaveChanges();
                        }
                    }
                }
            }

            List<User> users = _context2.ApplicationUsers.
                Include(comp => comp.FriendRequests).
                Include(comp => comp.FlightRequests).
                Include(comp => comp.SentRequests)
                .Include(comp => comp.Friends)
                .Include(comp => comp.Notifications)
                 .Include(comp => comp.Flights).ThenInclude(f => f.Flight)
                 .Include(comp => comp.Flights).ThenInclude(f => f.Flight).ThenInclude(f => f.Luggage)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.From)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.To)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Stops)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Seats).ThenInclude(d => d.Traveller)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                 .ToList();

     
            var user = await _userManager.FindByIdAsync(tim.seat.Traveller.IdUser);

            if (user != null)
            {

                if (user.FlightRequests == null)
                    user.FlightRequests = new List<TicketInvitation>();

                for(int i = 0;i < user.FlightRequests.Count();i++)
                {
                    if (user.FlightRequests[i].Ticket.Id == tim.id)
                    {
                        user.FlightRequests.Remove(user.FlightRequests[i]);
                        _context2.SaveChanges();
                    }
                    else
                    {
                        ret.Add(new TicketListingInfoModel()
                        {
                            seat = user.FlightRequests[i].Ticket.Seat,
                            discount = user.FlightRequests[i].Ticket.Discount,
                            id = user.FlightRequests[i].Ticket.Id,
                            flight = new FlightTicket()
                            {
                                departureDate = user.FlightRequests[i].Ticket.Flight.DepartureDate,
                                duration = user.FlightRequests[i].Ticket.Flight.Duration,
                                from = new DestinationListingInfoModel()
                                {
                                    name = user.FlightRequests[i].Ticket.Flight.From.Name,
                                    description = user.FlightRequests[i].Ticket.Flight.From.Description,
                                    id = user.FlightRequests[i].Ticket.Flight.From.Id,
                                    img = user.FlightRequests[i].Ticket.Flight.From.Img,
                                },
                                to = new DestinationListingInfoModel()
                                {
                                    name = user.FlightRequests[i].Ticket.Flight.To.Name,
                                    description = user.FlightRequests[i].Ticket.Flight.To.Description,
                                    id = user.FlightRequests[i].Ticket.Flight.To.Id,
                                    img = user.FlightRequests[i].Ticket.Flight.To.Img,
                                },
                                id = user.FlightRequests[i].Ticket.Flight.Id,
                                idCompany = user.FlightRequests[i].Ticket.Flight.IdCompany,
                                prise = user.FlightRequests[i].Ticket.Flight.Price,
                                trip = user.FlightRequests[i].Ticket.Flight.Trip
                            }

                        });
                    }
                }
            }
        
            return ret;
        }
        [HttpPost]
        [Route("CancelFlight")]
        public async Task<Object> CancelFlight(TicketListingInfoModel tim)
        {

            List<TicketListingInfoModel> ret = new List<TicketListingInfoModel>();
            List<Airline> companies = _context2.AirlineCompanies.Include(comp => comp.Flights).ThenInclude(d => d.Seats).ToList();

            foreach (var u in companies)
            {
                foreach (var u2 in u.Flights)
                {
                    foreach (var u3 in u2.Seats)
                    {
                        if (tim.seat.Id == u3.Id)
                        {
                            u3.Taken = false;
                            _context2.SaveChanges();
                        }
                    }
                }
            }

            List<User> users = _context2.ApplicationUsers.
                Include(comp => comp.FriendRequests).
                Include(comp => comp.FlightRequests).
                Include(comp => comp.SentRequests)
                .Include(comp => comp.Friends)
                .Include(comp => comp.Notifications)
                 .Include(comp => comp.Flights).ThenInclude(f => f.Flight)
                 .Include(comp => comp.Flights).ThenInclude(f => f.Flight).ThenInclude(f => f.Luggage)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.From)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.To)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Stops)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Seats).ThenInclude(d => d.Traveller)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                 .ToList();


            var user = await _userManager.FindByIdAsync(tim.seat.Traveller.IdUser);

            if (user != null)
            {

                if (user.Flights == null)
                    user.Flights = new List<Ticket>();

                for (int i = 0; i < user.Flights.Count(); i++)
                {
                    if (user.Flights[i].Id == tim.id)
                    {
                        user.Flights.Remove(user.Flights[i]);
                        _context2.SaveChanges();
                    }
                    else
                    {
                        ret.Add(new TicketListingInfoModel()
                        {
                            seat = user.Flights[i].Seat,
                            discount = user.Flights[i].Discount,
                            id = user.Flights[i].Id,
                            flight = new FlightTicket()
                            {
                                departureDate = user.Flights[i].Flight.DepartureDate,
                                duration = user.Flights[i].Flight.Duration,
                                from = new DestinationListingInfoModel()
                                {
                                    name = user.Flights[i].Flight.From.Name,
                                    description = user.Flights[i].Flight.From.Description,
                                    id = user.Flights[i].Flight.From.Id,
                                    img = user.Flights[i].Flight.From.Img,
                                },
                                to = new DestinationListingInfoModel()
                                {
                                    name = user.Flights[i].Flight.To.Name,
                                    description = user.Flights[i].Flight.To.Description,
                                    id = user.Flights[i].Flight.To.Id,
                                    img = user.Flights[i].Flight.To.Img,
                                },
                                id = user.Flights[i].Flight.Id,
                                idCompany = user.Flights[i].Flight.IdCompany,
                                prise = user.Flights[i].Flight.Price,
                                trip = user.Flights[i].Flight.Trip
                            }

                        });
                    }
                }
            }

            return ret;
        }
        [HttpPost]
        [Route("AcceptFlightRequest")]
        public async Task<Object> AcceptFlightRequest(TicketListingInfoModel tim)
        {
            
            List<TicketListingInfoModel> ret = new List<TicketListingInfoModel>();


            List<User> users = _context2.ApplicationUsers.
                Include(comp => comp.FriendRequests).
                Include(comp => comp.FlightRequests).
                Include(comp => comp.SentRequests)
                .Include(comp => comp.Friends)
                .Include(comp => comp.Notifications)
                 .Include(comp => comp.Flights).ThenInclude(f => f.Flight)
                 .Include(comp => comp.Flights).ThenInclude(f => f.Flight).ThenInclude(f => f.Luggage)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.From)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.To)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Stops)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Seats).ThenInclude(d => d.Traveller)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                 .ToList();

     
            var user = await _userManager.FindByIdAsync(tim.seat.Traveller.IdUser);

            if (user != null)
            {

                if (user.FlightRequests == null)
                    user.FlightRequests = new List<TicketInvitation>();

                for(int i = 0;i < user.FlightRequests.Count();i++)
                {
                    if (user.FlightRequests[i].Ticket.Id == tim.id)
                    {
                        Ticket t = new Ticket()
                        {
                            Discount = user.FlightRequests[i].Ticket.Discount,
                            Flight = user.FlightRequests[i].Ticket.Flight,
                            Seat =
                         user.FlightRequests[i].Ticket.Seat
                        };
                        if (user.Points > 100)
                        {
                            t.Discount += 5;
                        }
                        else if (user.Points > 100)
                        {
                            t.Discount += 10;
                        }
                        if (user.Flights == null)
                            user.Flights = new List<Ticket>();

                        user.FlightRequests.Remove(user.FlightRequests[i]);
                        _context2.SaveChanges();

                        user.Flights.Add(t);
                        user.Points += 5;
                        _context2.SaveChanges();
                    }
                    else
                    {
                        ret.Add(new TicketListingInfoModel()
                        {
                            seat = user.FlightRequests[i].Ticket.Seat,
                            discount = user.FlightRequests[i].Ticket.Discount,
                            id = user.FlightRequests[i].Ticket.Id,
                            flight = new FlightTicket()
                            {
                                departureDate = user.FlightRequests[i].Ticket.Flight.DepartureDate,
                                duration = user.FlightRequests[i].Ticket.Flight.Duration,
                                from = new DestinationListingInfoModel()
                                {
                                    name = user.FlightRequests[i].Ticket.Flight.From.Name,
                                    description = user.FlightRequests[i].Ticket.Flight.From.Description,
                                    id = user.FlightRequests[i].Ticket.Flight.From.Id,
                                    img = user.FlightRequests[i].Ticket.Flight.From.Img,
                                },
                                to = new DestinationListingInfoModel()
                                {
                                    name = user.FlightRequests[i].Ticket.Flight.To.Name,
                                    description = user.FlightRequests[i].Ticket.Flight.To.Description,
                                    id = user.FlightRequests[i].Ticket.Flight.To.Id,
                                    img = user.FlightRequests[i].Ticket.Flight.To.Img,
                                },
                                id = user.FlightRequests[i].Ticket.Flight.Id,
                                idCompany = user.FlightRequests[i].Ticket.Flight.IdCompany,
                                prise = user.FlightRequests[i].Ticket.Flight.Price,
                                trip = user.FlightRequests[i].Ticket.Flight.Trip
                            }

                        });
                    }
                }
            }
        
            return ret;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("Finish")]
        public async Task<Object> Finish(FinishModel model)
        {
            var options = new DbContextOptionsBuilder<ApplicationUsersContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            List<Airline> companies = _context2.AirlineCompanies
                .Include(comp => comp.Flights).ThenInclude(f => f.Luggage)
                .Include(fs => fs.Flights).ThenInclude(d => d.From)
                .Include(fs => fs.Flights).ThenInclude(d => d.To)
                .Include(fs => fs.Flights).ThenInclude(d => d.Stops)
                .Include(fs => fs.Flights).ThenInclude(d => d.Raters)
                .Include(fs => fs.Flights).ThenInclude(d => d.SoldTickets)
                .Include(fs => fs.Flights).ThenInclude(d => d.Seats).ThenInclude(d => d.Traveller)
                .Include(fs => fs.Flights).ThenInclude(d => d.SoldTickets)
                .Include(fs => fs.Flights).ThenInclude(d => d.Raters)
                .ToList();


            var users = _context2.Users.Include(us => us.Discount);


            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userid);


            Discount dis = new Discount();

            foreach (var us in users)
            {
                var roles = await _userManager.GetRolesAsync(us);
                if (roles.Contains("WebsiteAdministrator"))
                {
                    dis = us.Discount;
                }
            }

            foreach (var u in companies)
            {
                foreach (var u2 in u.Flights)
                {
                    foreach (var u3 in u2.Seats)
                    {
                        if (u3.Id == model.seat.Id)
                        {
                            Ticket t = new Ticket() { Discount = 0, Flight = u2, Seat = u3 };

                            if (userid == model.seat.Traveller.IdUser)
                            {

                                if (user.Points > dis.BronzeTier && user.Points < dis.SilverTier)
                                {
                                    t.Discount = (Int32)(dis.DiscountPercent) + t.Discount;
                                }

                                else if (user.Points > dis.SilverTier && user.Points < dis.GoldTier)
                                    t.Discount = (Int32)(dis.DiscountPercent * 2) + t.Discount;
                                else
                                    t.Discount = (Int32)(dis.DiscountPercent * 3) + t.Discount;
                                user.Points += 5;
                                if (u2.SoldTickets == null)
                                    u2.SoldTickets = new List<Ticket>();
                                // u2.SoldTickets.Add(t);


                                if (user.Flights == null)
                                    user.Flights = new List<Ticket>();
                                user.Flights.Add(t);
                                _context2.Update(user);
                                _context2.SaveChanges();
                            }
                            else
                            {
                                var user2 = await _userManager.FindByIdAsync(model.seat.Traveller.IdUser);
                                if (user2 != null)
                                {
                                    if (user2.FlightRequests == null)
                                        user2.FlightRequests = new List<TicketInvitation>();
                                    user2.FlightRequests.Add(new TicketInvitation() { Ticket = t });
                                    _context2.Update(user);
                                    _context2.SaveChanges();
                                }
                            }

                        }
                    }
                }
            }

           


            return null;
        }

        [HttpPost]
        [Route("RateCompany")]
        public async Task<Object> RateCompany(RateCompanyModel model)
        {
            List<Airline> companies = _context2.AirlineCompanies
                .Include(comp => comp.FastTickets)
                .Include(comp => comp.Destinations)
                .Include(comp => comp.PopDestinaations)
                .Include(comp => comp.Raters)
                .Include(comp => comp.Flights).ThenInclude(f => f.Luggage)
                .Include(fs => fs.Flights).ThenInclude(d => d.From)
                .Include(fs => fs.Flights).ThenInclude(d => d.To)
                .Include(fs => fs.Flights).ThenInclude(d => d.Stops)
                .Include(fs => fs.Flights).ThenInclude(d => d.Raters)
                .Include(fs => fs.Flights).ThenInclude(d => d.SoldTickets)
                .Include(fs => fs.Flights).ThenInclude(d => d.Seats).ThenInclude(d => d.Traveller)
                .Include(fs => fs.Flights).ThenInclude(d => d.SoldTickets)
                .Include(fs => fs.Flights).ThenInclude(d => d.Raters)
                .ToList();


            UserModel um = new UserModel();
            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userid);

           
            foreach (var c in companies)
            {

                if (c.Name == model.compId)
                {
                    if (c.Raters == null)
                        c.Raters = new List<Rater>();
                    c.Raters.Add(new Rater() { Rate = model.id });
                    int suma = 0;
                    for (int i = 0; i < c.Raters.Count(); i++)
                    {
                        suma += c.Raters[i].Rate;
                    }
                    c.Rating = suma / c.Raters.Count(); 
                }
            }

            foreach (var t in user.Flights)
            {
                if (t.Flight.Id == model.flightiD)
                {
                    t.gaveRateCompany = true;
                }
            }

            _context2.SaveChanges();

            return Ok(new { message = "Success" });
        }

        [HttpPost]
        [Route("RateFlight")]
        public async Task<Object> RateFlight(RateFlightModel model)
        {
            List<Airline> companies = _context2.AirlineCompanies
                .Include(comp => comp.FastTickets)
                .Include(comp => comp.Destinations)
                .Include(comp => comp.PopDestinaations)
                .Include(comp => comp.Raters)
                .Include(comp => comp.Flights).ThenInclude(f => f.Luggage)
                .Include(fs => fs.Flights).ThenInclude(d => d.From)
                .Include(fs => fs.Flights).ThenInclude(d => d.To)
                .Include(fs => fs.Flights).ThenInclude(d => d.Stops)
                .Include(fs => fs.Flights).ThenInclude(d => d.Raters)
                .Include(fs => fs.Flights).ThenInclude(d => d.SoldTickets)
                .Include(fs => fs.Flights).ThenInclude(d => d.Seats).ThenInclude(d => d.Traveller)
                .Include(fs => fs.Flights).ThenInclude(d => d.SoldTickets)
                .Include(fs => fs.Flights).ThenInclude(d => d.Raters)
                .ToList();


            UserModel um = new UserModel();
            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userid);

            foreach (var c in companies)
            {
                foreach(var f in c.Flights)
                {
                    if(f.Id == model.flight)
                    {
                        if (f.Raters == null)
                            f.Raters = new List<Rater>();
                        f.Raters.Add(new Rater() { Rate = model.id });
                        int suma = 0;
                        for(int i = 0; i < f.Raters.Count();i++)
                        {
                            suma += f.Raters[i].Rate;
                        }
                        f.Rate = suma + c.Raters.Count();
                    }
                }
            }
            foreach (var t in user.Flights)
            {
                if (t.Flight.Id == model.flight)
                {
                    t.gaveRate = true;
                }
            }
            _context2.SaveChanges();

            return Ok(new { message = "Success" });
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("LoadUser")]
        //POST : /api/ApplicationUser/Register
        public async Task<Object> LoadUser()
        {
            List<User> users = _context2.ApplicationUsers
                .Include(us=>us.RentedCars).ThenInclude(car=>car.Dates)
                .Include(comp => comp.FriendRequests).
                 Include(comp => comp.FlightRequests).                 
                Include(comp => comp.SentRequests)
                .Include(comp => comp.Friends)
                .Include(comp => comp.Notifications)
                 .Include(comp => comp.Flights).ThenInclude(f => f.Flight).ThenInclude(f => f.Luggage)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.From)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.To)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Stops)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Seats).ThenInclude(d => d.Traveller)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                 .ToList();

            UserModel um = new UserModel();
            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userid);
            um.FullName = user.FullName;
            um.Address = user.Address;
            um.Birthday = user.Birthday.ToString();
            um.Passport = user.Passport;
            um.changedPassword = user.ChangedPassword;
            um.OldFlights = new List<TicketListingInfoModel>();
            um.socialUser = user.SocialUser;

            um.Flights = new List<TicketListingInfoModel>();
            if (user.Flights == null)
                user.Flights = new List<Ticket>();
            foreach(var of in user.Flights)
            {
                if(DateTime.Parse(of.Flight.DepartureDate) < DateTime.Now)
                {
                    um.OldFlights.Add(new TicketListingInfoModel()
                    {
                        seat = of.Seat,
                        discount = of.Discount,
                        id = of.Id,
                        gaveRate = of.gaveRate,
                        gaveRateCompany = of.gaveRateCompany,
                        flight = new FlightTicket()
                        {
                            departureDate = of.Flight.DepartureDate,
                            duration = of.Flight.Duration,
                            id = of.Flight.Id,

                            trip = of.Flight.Trip,
                            prise = of.Flight.Price,
                            from = new DestinationListingInfoModel()
                            {
                                id = of.Flight.From.Id,
                                description = of.Flight.From.Description,
                                img = of.Flight.From.Img,
                                name = of.Flight.From.Name
                            },
                            to = new DestinationListingInfoModel()
                            {
                                id = of.Flight.To.Id,
                                description = of.Flight.To.Description,
                                img = of.Flight.To.Img,
                                name = of.Flight.To.Name
                            }
    ,
                            idCompany = of.Flight.IdCompany
                        }
                    });
                }
                else
                {
                    um.Flights.Add(new TicketListingInfoModel()
                    {
                        seat = of.Seat,
                        discount = of.Discount,
                        id = of.Id,
                        gaveRate = of.gaveRate,
                        gaveRateCompany = of.gaveRateCompany,
                        flight = new FlightTicket()
                        {
                            departureDate = of.Flight.DepartureDate,
                            duration = of.Flight.Duration,
                            id = of.Flight.Id,
                            trip = of.Flight.Trip,
                            prise = of.Flight.Price,
                            from = new DestinationListingInfoModel()
                            {
                                id = of.Flight.From.Id,
                                description = of.Flight.From.Description,
                                img = of.Flight.From.Img,
                                name = of.Flight.From.Name
                            },
                            to = new DestinationListingInfoModel()
                            {
                                id = of.Flight.To.Id,
                                description = of.Flight.To.Description,
                                img = of.Flight.To.Img,
                                name = of.Flight.To.Name
                            }
    ,
                            idCompany = of.Flight.IdCompany
                        }
                    });
                }


            }

            um.Id = user.Id;
            um.Username = user.UserName;
            um.Points = user.Points;
            um.Email = user.Email;
            um.CompanyId = user.CompanyId;

            um.FlightRequests = new List<TicketListingInfoModel>();
            um.Friends = new List<UserModel>();
            um.SentRequests = new List<FriendRequestSentModel>();
            um.Notifications = new List<Notification>();
            um.FriendRequests = new List<FriendRequestReceivedModel>();

            if (user.FlightRequests == null)
                user.FlightRequests = new List<TicketInvitation>();
            foreach (var fr in user.FlightRequests)
            {
                um.FlightRequests.Add(new TicketListingInfoModel()
                {
                    seat = fr.Ticket.Seat,
                    discount = fr.Ticket.Discount,
                    id = fr.Ticket.Id,
                    flight = new FlightTicket()
                    {
                        departureDate = fr.Ticket.Flight.DepartureDate,
                        duration = fr.Ticket.Flight.Duration,
                        from = new DestinationListingInfoModel()
                        {
                            name = fr.Ticket.Flight.From.Name,
                            description = fr.Ticket.Flight.From.Description,
                            id = fr.Ticket.Flight.From.Id,
                            img = fr.Ticket.Flight.From.Img,
                        },
                        to = new DestinationListingInfoModel()
                        {
                            name = fr.Ticket.Flight.To.Name,
                            description = fr.Ticket.Flight.To.Description,
                            id = fr.Ticket.Flight.To.Id,
                            img = fr.Ticket.Flight.To.Img,
                        },
                        id = fr.Ticket.Flight.Id,
                        idCompany = fr.Ticket.Flight.IdCompany,
                        prise = fr.Ticket.Flight.Price,
                        trip = fr.Ticket.Flight.Trip
                    }

                });
            }
            if (user.Friends == null)
                user.Friends = new List<User>();
            foreach (var fr in user.Friends)
            {
                um.Friends.Add(new UserModel() {Address = fr.Address, Email = fr.Email, Birthday = fr.Birthday.ToString(),
                FullName = fr.FullName, Username = fr.UserName, Id = fr.Id, Passport = fr.Passport});
            }
            if (user.SentRequests == null)
                user.SentRequests = new List<FriendRequestSent>();
            foreach (var fr in user.SentRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.SentRequests.Add(new FriendRequestSentModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.FriendRequests == null)
                user.FriendRequests = new List<FriendRequestReceived>();
            foreach (var fr in user.FriendRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.FriendRequests.Add(new FriendRequestReceivedModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.Notifications == null)
                user.Notifications = new List<Notification>();
            foreach (var fr in user.Notifications)
            {
                um.Notifications.Add(new Notification() { Id = fr.Id, Text = fr.Text });
            }

            var companies = _context2.RentACarCompanies
                .Include(comp => comp.Cars)
                .Include(comp => comp.Reservations).ThenInclude(res => res.Extras)
                .ToList();

            um.RentedCars = new List<CarReservationModel>();
            foreach (var res in user.RentedCars)
            {
                string companyName = "";
                int companyId = 0;
                foreach (var comp in companies)
                {
                    if (comp.Reservations.Contains(res))
                    {
                        companyName = comp.Name;
                        companyId = comp.ID;
                    }
                }
                um.RentedCars.Add(new CarReservationModel()
                {
                    car = new CarModel()
                    {
                        avrageRating = res.Car.AvrageRating,
                        brand = res.Car.Brand,
                        id = res.Car.ID,
                        image = res.Car.Image,
                        location = res.Car.Location,
                        model = res.Car.Model,
                        passengers = res.Car.Passengers,
                        price = res.Car.PricePerDay,
                        type = res.Car.Type,
                        year = res.Car.Year
                    },
                    company = companyName,
                    companyId = companyId,
                    dateFrom = res.Dates[0].DateStr,
                    dateTo = res.Dates[res.Dates.Count - 1].DateStr,
                    dropOffLocation = res.ReturnLocation,
                    dropOffTime = res.ReturnTime,
                    id = res.Id,
                    pickUpLocation = res.PickUpLocation,
                    pickUpTime = res.PickUpTime,
                    ratedCar = res.RatedCar,
                    ratedCompany = res.RatedCompany,
                    timeStamp = res.TimeStamp.ToShortDateString(),
                    total = res.TotalPrice,
                    Extras = res.Extras
                });
            }

            return um;
        }

        [HttpPost]
        [Route("LoadUserById")]
        //POST : /api/ApplicationUser/Register
        public async Task<Object> LoadUserById(SearchUserModel id)
        {
            List<User> users = _context2.ApplicationUsers.
                Include(comp => comp.FriendRequests).
                 
                Include(comp => comp.SentRequests)
                .Include(comp => comp.Friends)
                .Include(comp => comp.Notifications)
                 .Include(comp => comp.Flights).ThenInclude(f => f.Flight)
                 .Include(comp => comp.Flights).ThenInclude(f => f.Flight).ThenInclude(f => f.Luggage)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.From)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.To)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Stops)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Seats).ThenInclude(d => d.Traveller)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                 .ToList();

            UserModel um = new UserModel();

            var user = await _userManager.FindByIdAsync(id.passport);


            if (user.FlightRequests == null)
                user.FlightRequests = new List<TicketInvitation>();
            foreach (var fr in user.FlightRequests)
            {
                um.FlightRequests.Add(new TicketListingInfoModel()
                {
                    seat = fr.Ticket.Seat,
                    discount = fr.Ticket.Discount,
                    id = fr.Ticket.Id,
                    flight = new FlightTicket()
                    {
                        departureDate = fr.Ticket.Flight.DepartureDate,
                        duration = fr.Ticket.Flight.Duration,
                        from = new DestinationListingInfoModel()
                        {
                            name = fr.Ticket.Flight.From.Name,
                            description = fr.Ticket.Flight.From.Description,
                            id = fr.Ticket.Flight.From.Id,
                            img = fr.Ticket.Flight.From.Img,
                        },
                        to = new DestinationListingInfoModel()
                        {
                            name = fr.Ticket.Flight.To.Name,
                            description = fr.Ticket.Flight.To.Description,
                            id = fr.Ticket.Flight.To.Id,
                            img = fr.Ticket.Flight.To.Img,
                        },
                        id = fr.Ticket.Flight.Id,
                        idCompany = fr.Ticket.Flight.IdCompany,
                        prise = fr.Ticket.Flight.Price,
                        trip = fr.Ticket.Flight.Trip
                    }

                });
            }

            um.FullName = user.FullName;
            um.Address = user.Address;
            um.Birthday = user.Birthday.ToString();
            um.Passport = user.Passport;
            um.changedPassword = user.ChangedPassword;
          
            um.Id = user.Id;
            um.Username = user.UserName;
            um.Points = user.Points;
            um.Email = user.Email;
            um.CompanyId = user.CompanyId;

            return um;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        [Route("LoadPeople")]
        //POST : /api/ApplicationUser/Register
        public async Task<Object> LoadPeople()
        {
            List<UserModel> ret = new List<UserModel>();
            List<User> users = _context2.ApplicationUsers.
                Include(comp => comp.FriendRequests).

                Include(comp => comp.SentRequests)
                .Include(comp => comp.Friends)
             
                .Include(comp => comp.Notifications)
                 .Include(comp => comp.Flights).ThenInclude(f => f.Flight).ThenInclude(f => f.Luggage)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.From)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.To)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Stops)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Seats).ThenInclude(d => d.Traveller)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                 .ToList();


            foreach (var usermodel in users)
            {
                bool uZahtjevima = false;
                bool uZahtjevima2 = false;
                bool reg = false;
                UserModel um = new UserModel();
                string userid = User.Claims.First(c => c.Type == "UserID").Value;
                var user = await _userManager.FindByIdAsync(userid);
                List<FriendRequestSent> frs = new List<FriendRequestSent>();
                if (user.SentRequests == null)
                    user.SentRequests = new List<FriendRequestSent>();
                foreach (var fr in user.SentRequests)
                {
                    if (fr.User == usermodel.Id)
                    {
                        uZahtjevima = true;
                    }
                }

                if (user.FriendRequests == null)
                    user.FriendRequests = new List<FriendRequestReceived>();
                foreach (var fr in user.FriendRequests)
                {
                    if (fr.User == usermodel.Id)
                    {
                        uZahtjevima2 = true;
                    }
                }
                var userIdentity = await _userManager.IsInRoleAsync(usermodel, "RegisteredUser");
                if (!userIdentity)
                {
                    reg = true;
                }
                if (user.Friends == null)
                    user.Friends = new List<User>();
                if (userid != usermodel.Id && !reg && !user.Friends.Contains(usermodel) && !uZahtjevima && !uZahtjevima2)
                {
                    um.FullName = usermodel.FullName;
                    um.Address = usermodel.Address;
                    um.Birthday = usermodel.Birthday.ToString();
                    um.Passport = usermodel.Passport;


                    um.Id = usermodel.Id;
                    um.Username = usermodel.UserName;
                    um.Points = usermodel.Points;
                    um.Email = usermodel.Email;
                    um.CompanyId = usermodel.CompanyId;
                    ret.Add(um);
                }
            }
            return ret;
        }

        [HttpPost]
        [Route("SendRequest")]
        //POST : /api/ApplicationUser/Register
        public async Task<Object> SendRequest(SendRequestModel model)
        {
            List<UserModel> ret = new List<UserModel>();
            List<User> users = _context2.ApplicationUsers.
                   Include(comp => comp.FriendRequests).
                    Include(comp => comp.FlightRequests).
                   Include(comp => comp.SentRequests)
                   .Include(comp => comp.Friends)
                   .Include(comp => comp.Notifications)
                    .Include(comp => comp.Flights).ThenInclude(f => f.Flight).ThenInclude(f => f.Luggage)
                    .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.From)
                    .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.To)
                    .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Stops)
                    .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                    .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                    .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Seats).ThenInclude(d => d.Traveller)
                    .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                    .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                    .ToList();

            User reciver = await _userManager.FindByIdAsync(model.idTo);
            User sender = await _userManager.FindByIdAsync(model.idFrom);

            if (sender.SentRequests == null)
                sender.SentRequests = new List<FriendRequestSent>();
            sender.SentRequests.Add(new FriendRequestSent()
            {
                User = reciver.Id

            });

            if (reciver.FriendRequests == null)
                reciver.FriendRequests = new List<FriendRequestReceived>();
            reciver.FriendRequests.Add(new FriendRequestReceived()
            {
                User = sender.Id

            });
            _context2.Update(reciver); 
            _context2.Update(sender);
            _context2.SaveChanges();

            foreach (var usermodel in users)
            {

                UserModel um = new UserModel();


                if (model.idFrom != usermodel.Id && model.idTo != usermodel.Id)
                {
                    um.FullName = usermodel.FullName;
                    um.Address = usermodel.Address;
                    um.Birthday = usermodel.Birthday.ToString();
                    um.Passport = usermodel.Passport;

                    um.Id = usermodel.Id;
                    um.Username = usermodel.UserName;
                    um.Points = usermodel.Points;
                    um.Email = usermodel.Email;
                    um.CompanyId = usermodel.CompanyId;

                    um.FlightRequests = new List<TicketListingInfoModel>();
                    um.Friends = new List<UserModel>();
                    um.SentRequests = new List<FriendRequestSentModel>();
                    um.Notifications = new List<Notification>();
                    um.FriendRequests = new List<FriendRequestReceivedModel>();

                    if (usermodel.Friends == null)
                        usermodel.Friends = new List<User>();
                    foreach (var fr in usermodel.Friends)
                    {
                        um.Friends.Add(new UserModel()
                        {
                            Address = fr.Address,
                            Email = fr.Email,
                            Birthday = fr.Birthday.ToString(),
                            FullName = fr.FullName,
                            Username = fr.UserName,
                            Id = fr.Id,
                            Passport = fr.Passport
                        });
                    }
                    if (usermodel.SentRequests == null)
                        usermodel.SentRequests = new List<FriendRequestSent>();
                    foreach (var fr in usermodel.SentRequests)
                    {
                        User pom = await _userManager.FindByIdAsync(fr.User);
                        um.SentRequests.Add(new FriendRequestSentModel()
                        {
                            Id = fr.Id,
                            User = new UserModel()
                            {
                                Address = pom.Address,
                                Email = pom.Email,
                                Birthday = pom.Birthday.ToString(),
                                FullName = pom.FullName,
                                Username = pom.UserName,
                                Id = pom.Id,
                                Passport = pom.Passport
                            }
                        });
                    }
                    if (usermodel.FriendRequests == null)
                        usermodel.FriendRequests = new List<FriendRequestReceived>();
                    foreach (var fr in usermodel.FriendRequests)
                    {
                        User pom = await _userManager.FindByIdAsync(fr.User);
                        um.FriendRequests.Add(new FriendRequestReceivedModel()
                        {
                            Id = fr.Id,
                            User = new UserModel()
                            {
                                Address = pom.Address,
                                Email = pom.Email,
                                Birthday = pom.Birthday.ToString(),
                                FullName = pom.FullName,
                                Username = pom.UserName,
                                Id = pom.Id,
                                Passport = pom.Passport
                            }
                        });


                        ret.Add(um);
                    }
                }
            }
            return ret;
        }

        [HttpPost]
        [Route("AcceptRequest")]
        //POST : /api/ApplicationUser/Register
        public async Task<Object> AcceptRequest(SendRequestModel model)
        {

            List<User> users = _context2.ApplicationUsers.
                Include(comp => comp.FriendRequests).
                 Include(comp => comp.FlightRequests).
                Include(comp => comp.SentRequests)
                .Include(comp => comp.Friends)
                .Include(comp => comp.Notifications)
                 .Include(comp => comp.Flights).ThenInclude(f => f.Flight).ThenInclude(f => f.Luggage)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.From)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.To)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Stops)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Seats).ThenInclude(d => d.Traveller)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                 .ToList();

            User reciver = await _userManager.FindByIdAsync(model.idTo);
            User sender = await _userManager.FindByIdAsync(model.idFrom);

            if (sender.Friends == null)
                sender.Friends = new List<User>();
            if (sender.FriendRequests == null)
                sender.FriendRequests = new List<FriendRequestReceived>();
            if (reciver.Friends == null)
                reciver.Friends = new List<User>();
            if (reciver.SentRequests == null)
                reciver.SentRequests = new List<FriendRequestSent>();
            if (reciver.Notifications == null)
                reciver.Notifications = new List<Notification>();
            for (int i = 0;i < sender.FriendRequests.Count();i++)
            {
                if(sender.FriendRequests[i].User == reciver.Id)
                {
                    sender.Friends.Add(reciver);
                    sender.FriendRequests.Remove(sender.FriendRequests[i]);
                }
            }

            for (int i = 0; i < reciver.SentRequests.Count(); i++)
            {
                if (reciver.SentRequests[i].User == sender.Id)
                {
                    reciver.Friends.Add(sender);
                    reciver.Notifications.Add(new Notification() { Text = "User " + sender.FullName + " accepted your friend request!" });
                    reciver.SentRequests.Remove(reciver.SentRequests[i]);
                }
            }
            _context2.Update(reciver);
            _context2.Update(sender);
            _context2.SaveChanges();

            UserModel um = new UserModel();
            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userid);
            um.FullName = user.FullName;
            um.Address = user.Address;
            um.Birthday = user.Birthday.ToString();
            um.Passport = user.Passport;
            um.OldFlights = new List<TicketListingInfoModel>();
            um.Flights = new List<TicketListingInfoModel>();
            if (user.Flights == null)
                user.Flights = new List<Ticket>();
            foreach (var of in user.Flights)
            {

                um.Flights.Add(new TicketListingInfoModel()
                {
                    seat = of.Seat,
                    discount = of.Discount,
                    id = of.Id
                ,
                    flight = new FlightTicket()
                    {
                        departureDate = of.Flight.DepartureDate,
                        duration = of.Flight.Duration,
                        id = of.Flight.Id,
                        trip = of.Flight.Trip,
                        prise = of.Flight.Price,
                        from = new DestinationListingInfoModel()
                        {
                            id = of.Flight.From.Id,
                            description = of.Flight.From.Description,
                            img = of.Flight.From.Img,
                            name = of.Flight.From.Name
                        },
                        to = new DestinationListingInfoModel()
                        {
                            id = of.Flight.To.Id,
                            description = of.Flight.To.Description,
                            img = of.Flight.To.Img,
                            name = of.Flight.To.Name
                        }
                    ,
                        idCompany = of.Flight.IdCompany
                    }
                });

            }

            um.Id = user.Id;
            um.Username = user.UserName;
            um.Points = user.Points;
            um.Email = user.Email;
            um.CompanyId = user.CompanyId;

            um.FlightRequests = new List<TicketListingInfoModel>();
            um.Friends = new List<UserModel>();
            um.SentRequests = new List<FriendRequestSentModel>();
            um.Notifications = new List<Notification>();
            um.FriendRequests = new List<FriendRequestReceivedModel>();

            if (user.FlightRequests == null)
                user.FlightRequests = new List<TicketInvitation>();
            foreach (var fr in user.FlightRequests)
            {
                um.FlightRequests.Add(new TicketListingInfoModel()
                {
                    seat = fr.Ticket.Seat,
                    discount = fr.Ticket.Discount,
                    id = fr.Ticket.Id,
                    flight = new FlightTicket()
                    {
                        departureDate = fr.Ticket.Flight.DepartureDate,
                        duration = fr.Ticket.Flight.Duration,
                        from = new DestinationListingInfoModel()
                        {
                            name = fr.Ticket.Flight.From.Name,
                            description = fr.Ticket.Flight.From.Description,
                            id = fr.Ticket.Flight.From.Id,
                            img = fr.Ticket.Flight.From.Img,
                        },
                        to = new DestinationListingInfoModel()
                        {
                            name = fr.Ticket.Flight.To.Name,
                            description = fr.Ticket.Flight.To.Description,
                            id = fr.Ticket.Flight.To.Id,
                            img = fr.Ticket.Flight.To.Img,
                        },
                        id = fr.Ticket.Flight.Id,
                        idCompany = fr.Ticket.Flight.IdCompany,
                        prise = fr.Ticket.Flight.Price,
                        trip = fr.Ticket.Flight.Trip
                    }

                });
            }
            if (user.Friends == null)
                user.Friends = new List<User>();
            foreach (var fr in user.Friends)
            {
                um.Friends.Add(new UserModel()
                {
                    Address = fr.Address,
                    Email = fr.Email,
                    Birthday = fr.Birthday.ToString(),
                    FullName = fr.FullName,
                    Username = fr.UserName,
                    Id = fr.Id,
                    Passport = fr.Passport
                });
            }
            if (user.SentRequests == null)
                user.SentRequests = new List<FriendRequestSent>();
            foreach (var fr in user.SentRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.SentRequests.Add(new FriendRequestSentModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.FriendRequests == null)
                user.FriendRequests = new List<FriendRequestReceived>();
            foreach (var fr in user.FriendRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.FriendRequests.Add(new FriendRequestReceivedModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.Notifications == null)
                user.Notifications = new List<Notification>();
            foreach (var fr in user.Notifications)
            {
                um.Notifications.Add(new Notification() { Id = fr.Id, Text = fr.Text });
            }

            return um;
        }

        [HttpPost]
        [Route("CancelRequest")]
        //POST : /api/ApplicationUser/Register
        public async Task<Object> CancelRequest(SendRequestModel model)
        {

            List<User> users = _context2.ApplicationUsers.
                Include(comp => comp.FriendRequests).
                 Include(comp => comp.FlightRequests).
                Include(comp => comp.SentRequests)
                .Include(comp => comp.Friends)
                .Include(comp => comp.Notifications)
                 .Include(comp => comp.Flights).ThenInclude(f => f.Flight).ThenInclude(f => f.Luggage)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.From)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.To)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Stops)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Seats).ThenInclude(d => d.Traveller)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                 .ToList();

            User reciver = await _userManager.FindByIdAsync(model.idTo);
            User sender = await _userManager.FindByIdAsync(model.idFrom);

            if (sender.Friends == null)
                sender.Friends = new List<User>();
            if (sender.FriendRequests == null)
                sender.FriendRequests = new List<FriendRequestReceived>();
            if (reciver.Friends == null)
                reciver.Friends = new List<User>();
            if (reciver.SentRequests == null)
                reciver.SentRequests = new List<FriendRequestSent>();
            if (reciver.Notifications == null)
                reciver.Notifications = new List<Notification>();
            for (int i = 0; i < sender.FriendRequests.Count(); i++)
            {
                if (sender.FriendRequests[i].User == reciver.Id)
                {
                    sender.FriendRequests.Remove(sender.FriendRequests[i]);
                }
            }

            for (int i = 0; i < reciver.SentRequests.Count(); i++)
            {
                if (reciver.SentRequests[i].User == sender.Id)
                {
                    reciver.Notifications.Add(new Notification() { Text = "User " + sender.FullName + " canceled your friend request!" });
                    reciver.SentRequests.Remove(reciver.SentRequests[i]);
                }
            }
            _context2.Update(reciver);
            _context2.Update(sender);
            _context2.SaveChanges();

            UserModel um = new UserModel();
            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userid);
            um.FullName = user.FullName;
            um.Address = user.Address;
            um.Birthday = user.Birthday.ToString();
            um.Passport = user.Passport;
            um.OldFlights = new List<TicketListingInfoModel>();
            um.Flights = new List<TicketListingInfoModel>();
            if (user.Flights == null)
                user.Flights = new List<Ticket>();
            foreach (var of in user.Flights)
            {

                if (DateTime.Parse(of.Flight.DepartureDate) < DateTime.Now)
                {
                    um.OldFlights.Add(new TicketListingInfoModel()
                    {
                        seat = of.Seat,
                        discount = of.Discount,
                        id = of.Id
,
                        flight = new FlightTicket()
                        {
                            departureDate = of.Flight.DepartureDate,
                            duration = of.Flight.Duration,
                            id = of.Flight.Id,
                            trip = of.Flight.Trip,
                            prise = of.Flight.Price,
                            from = new DestinationListingInfoModel()
                            {
                                id = of.Flight.From.Id,
                                description = of.Flight.From.Description,
                                img = of.Flight.From.Img,
                                name = of.Flight.From.Name
                            },
                            to = new DestinationListingInfoModel()
                            {
                                id = of.Flight.To.Id,
                                description = of.Flight.To.Description,
                                img = of.Flight.To.Img,
                                name = of.Flight.To.Name
                            }
    ,
                            idCompany = of.Flight.IdCompany
                        }
                    });
                }
                else
                {
                    um.Flights.Add(new TicketListingInfoModel()
                    {
                        seat = of.Seat,
                        discount = of.Discount,
                        id = of.Id
,
                        flight = new FlightTicket()
                        {
                            departureDate = of.Flight.DepartureDate,
                            duration = of.Flight.Duration,
                            id = of.Flight.Id,
                            trip = of.Flight.Trip,
                            prise = of.Flight.Price,
                            from = new DestinationListingInfoModel()
                            {
                                id = of.Flight.From.Id,
                                description = of.Flight.From.Description,
                                img = of.Flight.From.Img,
                                name = of.Flight.From.Name
                            },
                            to = new DestinationListingInfoModel()
                            {
                                id = of.Flight.To.Id,
                                description = of.Flight.To.Description,
                                img = of.Flight.To.Img,
                                name = of.Flight.To.Name
                            }
    ,
                            idCompany = of.Flight.IdCompany
                        }
                    });
                }

            }

            um.Id = user.Id;
            um.Username = user.UserName;
            um.Points = user.Points;
            um.Email = user.Email;
            um.CompanyId = user.CompanyId;

            um.FlightRequests = new List<TicketListingInfoModel>();
            um.Friends = new List<UserModel>();
            um.SentRequests = new List<FriendRequestSentModel>();
            um.Notifications = new List<Notification>();
            um.FriendRequests = new List<FriendRequestReceivedModel>();

            if (user.FlightRequests == null)
                user.FlightRequests = new List<TicketInvitation>();
            foreach (var fr in user.FlightRequests)
            {
                um.FlightRequests.Add(new TicketListingInfoModel()
                {
                    seat = fr.Ticket.Seat,
                    discount = fr.Ticket.Discount,
                    id = fr.Ticket.Id,
                    flight = new FlightTicket()
                    {
                        departureDate = fr.Ticket.Flight.DepartureDate,
                        duration = fr.Ticket.Flight.Duration,
                        from = new DestinationListingInfoModel()
                        {
                            name = fr.Ticket.Flight.From.Name,
                            description = fr.Ticket.Flight.From.Description,
                            id = fr.Ticket.Flight.From.Id,
                            img = fr.Ticket.Flight.From.Img,
                        },
                        to = new DestinationListingInfoModel()
                        {
                            name = fr.Ticket.Flight.To.Name,
                            description = fr.Ticket.Flight.To.Description,
                            id = fr.Ticket.Flight.To.Id,
                            img = fr.Ticket.Flight.To.Img,
                        },
                        id = fr.Ticket.Flight.Id,
                        idCompany = fr.Ticket.Flight.IdCompany,
                        prise = fr.Ticket.Flight.Price,
                        trip = fr.Ticket.Flight.Trip
                    }

                });
            }
            if (user.Friends == null)
                user.Friends = new List<User>();
            foreach (var fr in user.Friends)
            {
                um.Friends.Add(new UserModel()
                {
                    Address = fr.Address,
                    Email = fr.Email,
                    Birthday = fr.Birthday.ToString(),
                    FullName = fr.FullName,
                    Username = fr.UserName,
                    Id = fr.Id,
                    Passport = fr.Passport
                });
            }
            if (user.SentRequests == null)
                user.SentRequests = new List<FriendRequestSent>();
            foreach (var fr in user.SentRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.SentRequests.Add(new FriendRequestSentModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.FriendRequests == null)
                user.FriendRequests = new List<FriendRequestReceived>();
            foreach (var fr in user.FriendRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.FriendRequests.Add(new FriendRequestReceivedModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.Notifications == null)
                user.Notifications = new List<Notification>();
            foreach (var fr in user.Notifications)
            {
                um.Notifications.Add(new Notification() { Id = fr.Id, Text = fr.Text });
            }

            return um;
        }

        [HttpPost]
        [Route("RemoveFriend")]
        //POST : /api/ApplicationUser/Register
        public async Task<Object> RemoveFriend(SendRequestModel model)
        {

            List<User> users = _context2.ApplicationUsers.
                Include(comp => comp.FriendRequests).
                 Include(comp => comp.FlightRequests).
                Include(comp => comp.SentRequests)
                .Include(comp => comp.Friends)
                .Include(comp => comp.Notifications)
                 .Include(comp => comp.Flights).ThenInclude(f => f.Flight).ThenInclude(f => f.Luggage)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.From)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.To)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Stops)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Seats).ThenInclude(d => d.Traveller)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.SoldTickets)
                 .Include(fs => fs.Flights).ThenInclude(f => f.Flight).ThenInclude(d => d.Raters)
                 .Include(comp => comp.RentedCars).ThenInclude(cr => cr.Car)
                 .Include(comp => comp.RentedCars).ThenInclude(cr => cr.Dates)
                 .Include(comp => comp.RentedCars).ThenInclude(cr => cr.Extras)
                 .ToList();

            User reciver = await _userManager.FindByIdAsync(model.idTo);
            User sender = await _userManager.FindByIdAsync(model.idFrom);

            if (sender.Friends == null)
                sender.Friends = new List<User>();
            if (sender.FriendRequests == null)
                sender.FriendRequests = new List<FriendRequestReceived>();
            if (reciver.Friends == null)
                reciver.Friends = new List<User>();
            if (reciver.SentRequests == null)
                reciver.SentRequests = new List<FriendRequestSent>();
            if (reciver.Notifications == null)
                reciver.Notifications = new List<Notification>();
            for (int i = 0; i < sender.Friends.Count(); i++)
            {
                if (sender.Friends[i].Id == reciver.Id)
                {
                    sender.Friends.Remove(sender.Friends[i]);
                }
            }

            for (int i = 0; i < reciver.Friends.Count(); i++)
            {
                if (reciver.Friends[i].Id == sender.Id)
                {
                    reciver.Friends.Remove(reciver.Friends[i]);
                    reciver.Notifications.Add(new Notification() { Text = "User " + sender.FullName + " removed you from friends!" });
                }
            }
            _context2.Update(reciver);
            _context2.Update(sender);
            _context2.SaveChanges();

            UserModel um = new UserModel();
            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userid);
            um.FullName = user.FullName;
            um.Address = user.Address;
            um.Birthday = user.Birthday.ToString();
            um.Passport = user.Passport;
            um.OldFlights = new List<TicketListingInfoModel>();
            um.Flights = new List<TicketListingInfoModel>();
            if (user.Flights == null)
                user.Flights = new List<Ticket>();
            foreach (var of in user.Flights)
            {

                if (DateTime.Parse(of.Flight.DepartureDate) < DateTime.Now)
                {
                    um.OldFlights.Add(new TicketListingInfoModel()
                    {
                        seat = of.Seat,
                        discount = of.Discount,
                        id = of.Id
,
                        flight = new FlightTicket()
                        {
                            departureDate = of.Flight.DepartureDate,
                            duration = of.Flight.Duration,
                            id = of.Flight.Id,
                            trip = of.Flight.Trip,
                            prise = of.Flight.Price,
                            from = new DestinationListingInfoModel()
                            {
                                id = of.Flight.From.Id,
                                description = of.Flight.From.Description,
                                img = of.Flight.From.Img,
                                name = of.Flight.From.Name
                            },
                            to = new DestinationListingInfoModel()
                            {
                                id = of.Flight.To.Id,
                                description = of.Flight.To.Description,
                                img = of.Flight.To.Img,
                                name = of.Flight.To.Name
                            }
    ,
                            idCompany = of.Flight.IdCompany
                        }
                    });
                }
                else
                {
                    um.Flights.Add(new TicketListingInfoModel()
                    {
                        seat = of.Seat,
                        discount = of.Discount,
                        id = of.Id
,
                        flight = new FlightTicket()
                        {
                            departureDate = of.Flight.DepartureDate,
                            duration = of.Flight.Duration,
                            id = of.Flight.Id,
                            trip = of.Flight.Trip,
                            prise = of.Flight.Price,
                            from = new DestinationListingInfoModel()
                            {
                                id = of.Flight.From.Id,
                                description = of.Flight.From.Description,
                                img = of.Flight.From.Img,
                                name = of.Flight.From.Name
                            },
                            to = new DestinationListingInfoModel()
                            {
                                id = of.Flight.To.Id,
                                description = of.Flight.To.Description,
                                img = of.Flight.To.Img,
                                name = of.Flight.To.Name
                            }
    ,
                            idCompany = of.Flight.IdCompany
                        }
                    });
                }

            }

            um.Id = user.Id;
            um.Username = user.UserName;
            um.Points = user.Points;
            um.Email = user.Email;
            um.CompanyId = user.CompanyId;

            um.FlightRequests = new List<TicketListingInfoModel>();
            um.Friends = new List<UserModel>();
            um.SentRequests = new List<FriendRequestSentModel>();
            um.Notifications = new List<Notification>();
            um.FriendRequests = new List<FriendRequestReceivedModel>();

            if (user.FlightRequests == null)
                user.FlightRequests = new List<TicketInvitation>();
            foreach (var fr in user.FlightRequests)
            {
                um.FlightRequests.Add(new TicketListingInfoModel()
                {
                    seat = fr.Ticket.Seat,
                    discount = fr.Ticket.Discount,
                    id = fr.Ticket.Id,
                    flight = new FlightTicket()
                    {
                        departureDate = fr.Ticket.Flight.DepartureDate,
                        duration = fr.Ticket.Flight.Duration,
                        from = new DestinationListingInfoModel()
                        {
                            name = fr.Ticket.Flight.From.Name,
                            description = fr.Ticket.Flight.From.Description,
                            id = fr.Ticket.Flight.From.Id,
                            img = fr.Ticket.Flight.From.Img,
                        },
                        to = new DestinationListingInfoModel()
                        {
                            name = fr.Ticket.Flight.To.Name,
                            description = fr.Ticket.Flight.To.Description,
                            id = fr.Ticket.Flight.To.Id,
                            img = fr.Ticket.Flight.To.Img,
                        },
                        id = fr.Ticket.Flight.Id,
                        idCompany = fr.Ticket.Flight.IdCompany,
                        prise = fr.Ticket.Flight.Price,
                        trip = fr.Ticket.Flight.Trip
                    }

                });
            }
            if (user.Friends == null)
                user.Friends = new List<User>();
            foreach (var fr in user.Friends)
            {
                um.Friends.Add(new UserModel()
                {
                    Address = fr.Address,
                    Email = fr.Email,
                    Birthday = fr.Birthday.ToString(),
                    FullName = fr.FullName,
                    Username = fr.UserName,
                    Id = fr.Id,
                    Passport = fr.Passport
                });
            }
            if (user.SentRequests == null)
                user.SentRequests = new List<FriendRequestSent>();
            foreach (var fr in user.SentRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.SentRequests.Add(new FriendRequestSentModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.FriendRequests == null)
                user.FriendRequests = new List<FriendRequestReceived>();
            foreach (var fr in user.FriendRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.FriendRequests.Add(new FriendRequestReceivedModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.Notifications == null)
                user.Notifications = new List<Notification>();
            foreach (var fr in user.Notifications)
            {
                um.Notifications.Add(new Notification() { Id = fr.Id, Text = fr.Text });
            }
            return um;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("RentCar")]
        public async Task<object> RentCar(ReservationModel reservation)
        {
            var options = new DbContextOptionsBuilder<ApplicationUsersContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if(userRole != "RegisteredUser")
            {
                return BadRequest(new { message = "Forbidden action for this role." });
            }
            var user = await _userManager.FindByIdAsync(userId);

            if(!user.EmailConfirmed)
            {
                return BadRequest(new { message = "Please verify your email." });
            }

            using (var context = new ApplicationUsersContext(options))
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    context.RentACarCompanies
                        .Include(comp => comp.MainLocation)
                        .Include(comp => comp.Locations)
                        .Include(comp => comp.Cars)
                            .ThenInclude(car => car.RentedDates)
                        .Include(comp => comp.Cars)
                            .ThenInclude(car => car.Ratings)
                        .Include(comp => comp.Ratings)
                        .Include(comp => comp.QuickReservations)
                            .ThenInclude(qr => qr.Dates)
                        .Include(comp => comp.extras)
                        .ToList();

                    var company = context.RentACarCompanies.Find(reservation.company);
                    var car = context.RentACarCompanies.Find(reservation.company).Cars.Find(car => car.ID == reservation.car);
                    if (car.Removed)
                        return BadRequest(new { message = "Car is no longer available. Please choose a different vehicle." });

                    if (CheckAvailability(car, reservation.from, reservation.to))
                    {
                        if (!reservation.quickRes)
                        {
                            if (!CheckQuickReservations(company, car, reservation.from, reservation.to))
                                return BadRequest(new { message = "Car is no longer available. Please choose a different vehicle." });
                        }
                        CarReservation res = new CarReservation();
                        res.PickUpLocation = reservation.pickUpAddr;
                        res.ReturnLocation = reservation.dropOffAddr;
                        res.PickUpTime = reservation.fromTime;
                        res.ReturnTime = reservation.toTime;
                        res.RatedCar = 0;
                        res.RatedCompany = 0;
                        res.Car = car;
                        res.Dates = GetDates(reservation.from, reservation.to);
                        res.Extras = new List<ExtraAmenity>();
                        res.TimeStamp = DateTime.Now;
                        foreach (var am in company.extras)
                        {
                            if (reservation.extras.Contains(am.Id))
                                res.Extras.Add(am);
                        }

                        //formiranje cijene
                        var users = _context2.Users.Include(us => us.Discount);

                        Discount dis = new Discount();

                        foreach (var us in users)
                        {
                            var roles = await _userManager.GetRolesAsync(us);
                            if (roles.Contains("WebsiteAdministrator"))
                            {
                                dis = us.Discount;
                            }
                        }

                        res.TotalPrice = res.Dates.Count * res.Car.PricePerDay;

                        if (user.Points > dis.BronzeTier && user.Points < dis.SilverTier)
                            res.TotalPrice = res.TotalPrice - (res.TotalPrice * dis.DiscountPercent / 100);
                        else if (user.Points > dis.SilverTier && user.Points < dis.GoldTier)
                            res.TotalPrice = res.TotalPrice - (res.TotalPrice * (dis.DiscountPercent * 2) / 100);
                        else
                            res.TotalPrice = res.TotalPrice - (res.TotalPrice * (dis.DiscountPercent * 3) / 100);

                        foreach (var ex in res.Extras)
                        {
                            if (ex.OneTimePayment)
                                res.TotalPrice += ex.Price;
                            else
                                res.TotalPrice += ex.Price * res.Dates.Count;
                        }


                        if (company.Reservations == null)
                        {
                            company.Reservations = new List<CarReservation>();
                        }
                        company.Reservations.Add(res);
                        if (user.RentedCars == null)
                        {
                            user.RentedCars = new List<CarReservation>();
                        }
                        user.RentedCars.Add(res);
                        user.Points = user.Points + 5;
                        if (car.RentedDates == null)
                        {
                            car.RentedDates = new List<Date>();
                        }
                        foreach (var d in res.Dates)
                        {
                            car.RentedDates.Add(d);
                        }


                        context.Update(car);
                        try
                        {
                            await context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            await dbContextTransaction.RollbackAsync();
                            return BadRequest(new { message = "Car is no longer available. Please choose a different vehicle. - Conflict!" });
                        }
                        catch (Exception e)
                        {
                            await dbContextTransaction.RollbackAsync();
                            return BadRequest(new { message = "Car is no longer available. Please choose a different vehicle. - Conflict2!" });
                        }


                        context.Update(user);
                        try
                        {
                            await context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            await dbContextTransaction.RollbackAsync();
                            return BadRequest(new { message = "Car is no longer available. Please choose a different vehicle. - Conflict!" });
                        }
                        catch (Exception e)
                        {
                            await dbContextTransaction.RollbackAsync();
                            return BadRequest(new { message = "Car is no longer available. Please choose a different vehicle. - Conflict 2!" });
                        }

                        context.Update(company);
                        try
                        {
                            await context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            await dbContextTransaction.RollbackAsync();
                            return BadRequest(new { message = "Car is no longer available. Please choose a different vehicle. - Conflict!" });
                        }
                        catch (Exception e)
                        {
                            await dbContextTransaction.RollbackAsync();
                            return BadRequest(new { message = "Car is no longer available. Please choose a different vehicle. - Conflict 2!" });
                        }
                        await dbContextTransaction.CommitAsync();
                        return Ok();
                    }
                    else
                    {
                        return BadRequest(new { message = "Car is no longer available. Please choose a different vehicle." });
                    }
                } 
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("GiveUpCarReservation")]
        public async Task<object> GiveUpCarReservation(RemoveReservationModel model)
        {
            var options = new DbContextOptionsBuilder<ApplicationUsersContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            using (var context = new ApplicationUsersContext(options)) {
                using (var transaction = context.Database.BeginTransaction())
                {
                    var users = _context2.Users.Include(us => us.RentedCars).ThenInclude(rc => rc.Dates)
                        .Include(us => us.RentedCars).ThenInclude(rc => rc.Car)
                        .Include(us => us.RentedCars).ThenInclude(rc => rc.Extras).ToList();

                    string userId = User.Claims.First(c => c.Type == "UserID").Value;
                    string userRole = User.Claims.First(c => c.Type == "Roles").Value;
                    if (userRole != "RegisteredUser")
                    {
                        return "Forbbiden action for this role.";
                    }
                    var user = await _userManager.FindByIdAsync(userId);

                    var companies = context.RentACarCompanies
                    .Include(comp => comp.Cars).ThenInclude(car => car.RentedDates)
                    .Include(comp => comp.Reservations).ThenInclude(res => res.Car)
                    .Include(comp => comp.Reservations).ThenInclude(res => res.Dates)
                    .ToList();

                    var company = context.RentACarCompanies.Find(model.compId);
                    var reserv = company.Reservations.Find(res => res.Id == model.resId);

                    foreach (var car in company.Cars)
                    {
                        if (car.ID == reserv.Car.ID)
                        {
                            foreach (var date in reserv.Dates)
                            {
                                if (car.RentedDates.Contains(date))
                                    car.RentedDates.Remove(date);
                            }
                        }
                    }
                    //company.Reservations.Remove(reserv);
                    //context.Update(company);
                    //try
                    //{
                    //    await context.SaveChangesAsync();
                    //}
                    //catch (DbUpdateConcurrencyException)
                    //{
                    //    await transaction.RollbackAsync();
                    //    return BadRequest(new { message = "Reservation could not be removed from company" });
                    //}

                    //try
                    //{
                    //    user.RentedCars.Remove(reserv);
                    //    await _userManager.UpdateAsync(user);
                    //    context.Update(user);
                    //    await context.SaveChangesAsync();
                    //}
                    //catch (DbUpdateConcurrencyException)
                    //{
                    //    await transaction.RollbackAsync();
                    //    return BadRequest(new { message = "Reservation could not be removed" });
                    //}

                    context.Remove(reserv);
                    try
                    {
                        await context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(new { message = "Reservation could not be removed" });
                    }

                    await transaction.CommitAsync();

                    List<CarReservationModel> retList = new List<CarReservationModel>();
                    foreach (var res in user.RentedCars)
                    {
                        string companyName = "";
                        int companyId = 0;
                        foreach (var comp in companies)
                        {
                            if (comp.Reservations.Contains(res))
                            {
                                companyName = comp.Name;
                                companyId = comp.ID;
                            }
                        }
                        retList.Add(new CarReservationModel()
                        {
                            car = new CarModel()
                            {
                                avrageRating = res.Car.AvrageRating,
                                brand = res.Car.Brand,
                                id = res.Car.ID,
                                image = res.Car.Image,
                                location = res.Car.Location,
                                model = res.Car.Model,
                                passengers = res.Car.Passengers,
                                price = res.Car.PricePerDay,
                                type = res.Car.Type,
                                year = res.Car.Year
                            },
                            company = companyName,
                            companyId = companyId,
                            dateFrom = res.Dates[0].DateStr,
                            dateTo = res.Dates[res.Dates.Count - 1].DateStr,
                            dropOffLocation = res.ReturnLocation,
                            dropOffTime = res.ReturnTime,
                            id = res.Id,
                            pickUpLocation = res.PickUpLocation,
                            pickUpTime = res.PickUpTime,
                            ratedCar = res.RatedCar,
                            ratedCompany = res.RatedCompany,
                            timeStamp = res.TimeStamp.ToShortDateString(),
                            total = res.Dates.Count * res.Car.PricePerDay,
                            Extras = res.Extras
                        });
                        
                    }
                    return retList;
                }

            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("RateCarCompany")]
        public async Task<object> RateCarCompany(RateCarCompanyModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RegisteredUser")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);

            var companies = _context2.RentACarCompanies
                .Include(comp => comp.Cars).ThenInclude(car => car.RentedDates)
                .Include(comp => comp.Ratings)
                .Include(comp => comp.Reservations).ThenInclude(res => res.Car)
                .Include(comp => comp.Reservations).ThenInclude(res => res.Dates)
                .ToList();

            foreach(var comp in companies)
            {
                if(comp.ID==model.compId)
                {
                    if (comp.Ratings == null)
                        comp.Ratings = new List<Rating>();
                    comp.Ratings.Add(new Rating() { Rate = model.star });
                }
                int sum = 0;
                foreach(var r in comp.Ratings)
                {
                    sum += r.Rate;
                }
                comp.AvrageRating = sum / comp.Ratings.Count;
                foreach(var res in comp.Reservations)
                {
                    if(res.Id == model.resId)
                    {
                        res.RatedCompany = model.star;
                    }
                }
                _context2.Update(comp);
                _context2.SaveChanges();
            }

            return Ok();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("RateCar")]
        public async Task<object> RateCar(RateCarModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RegisteredUser")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);

            var companies = _context2.RentACarCompanies
                .Include(comp => comp.Cars).ThenInclude(car => car.Ratings)
                .Include(comp => comp.Reservations).ThenInclude(res => res.Car)
                .Include(comp => comp.Reservations).ThenInclude(res => res.Dates)
                .ToList();

            var comp = _context2.RentACarCompanies.Find(model.compId);
            var car = _context2.RentACarCompanies.Find(model.compId).Cars.Find(car => car.ID == model.carId);

            if (car.Ratings == null)
                car.Ratings = new List<Rating>();
            car.Ratings.Add(new Rating() { Rate = model.star });
            int sum = 0;
            foreach (var r in car.Ratings)
            {
                sum += r.Rate;
            }
            car.AvrageRating = sum / car.Ratings.Count;

            foreach (var res in comp.Reservations)
            {
                if (res.Id == model.resId)
                {
                    res.RatedCar = model.star;
                }
            }
            _context2.Update(comp);
            _context2.SaveChanges();
            return Ok();

        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("LoadCarRentalAdmin")]
        public async Task<object> LoadCarRentalAdmin()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);

            CarRentalAdminModel admin = new CarRentalAdminModel();
            admin.address = user.Address;
            admin.birthday = user.Birthday.ToShortDateString();
            admin.companyId = user.CompanyId;
            admin.email = user.Email;
            admin.fullName = user.FullName;
            admin.username = user.UserName;
            admin.verifiedEmail = true;
            admin.changedPassword = user.ChangedPassword;

            CompanyInfoModel model = new CompanyInfoModel();

            var companies = _context2.RentACarCompanies
                .Include(comp=>comp.Locations)
                .Include(comp=>comp.MainLocation)
                .Include(comp=>comp.Cars).ThenInclude(car=>car.RentedDates)
                .Include(comp=>comp.extras)
                .Include(comp => comp.QuickReservations).ThenInclude(qr => qr.DiscountedCar)
                .Include(comp=>comp.QuickReservations).ThenInclude(qr=>qr.Dates)
                .Include(comp=>comp.Reservations)
                .ToList();

            foreach (var comp in companies)
            {
                if (comp.ID == user.CompanyId)
                {
                    model.name = comp.Name;
                    model.description = comp.Description;
                    model.logo = comp.Image;
                    model.locations = new LocationsModel();
                    model.locations.mainLocation = comp.MainLocation;
                    model.locations.locations = new List<Address>();
                    model.activated = comp.Activated;
                    foreach(var loc in comp.Locations)
                    {
                        model.locations.locations.Add(loc);
                    }
                    model.ratings = new List<int>();
                    model.ratings.Add(0);
                    model.ratings.Add(0);
                    model.ratings.Add(0);
                    model.ratings.Add(0);
                    model.ratings.Add(0);
                    foreach(var res in comp.Reservations)
                    {
                        if(res.RatedCompany != 0)
                        {
                            model.ratings[res.RatedCompany - 1]++;
                        }
                    }

                    model.cars = new List<CarModelAdmin>();
                    foreach(var car in comp.Cars)
                    {
                        if (!car.Removed)
                        {
                            List<string> rented = new List<string>();
                            foreach (var date in car.RentedDates)
                            {
                                rented.Add(date.DateStr);
                            }
                            List<QuickReservationModel> discount = new List<QuickReservationModel>();
                            foreach (var res in comp.QuickReservations)
                            {
                                if (res.DiscountedCar.ID == car.ID)
                                {
                                    discount.Add(new QuickReservationModel() { id = res.Id, carId = res.DiscountedCar.ID, from = res.Dates[0].DateStr, to = res.Dates[res.Dates.Count - 1].DateStr });
                                }
                            }
                            model.cars.Add(new CarModelAdmin() { avrageRating = car.AvrageRating, brand = car.Brand, id = car.ID, image = car.Image, location = car.Location, model = car.Model, passengers = car.Passengers, price = car.PricePerDay, type = car.Type, year = car.Year, rentedDates = rented, quickReservations = discount });

                        }
                    }

                    List<ExtraAmenity> extras = new List<ExtraAmenity>();
                    extras = comp.extras;
                    model.extras = extras;

                    //find statistical data
                    //daily
                    List<int> dailyRes = new List<int>();
                    List<string> dailyLabels = new List<string>();
                    DateTime refDate = DateTime.Now;
                    for (int i = 0; i< 7; i++)
                    {
                        int count = 0;
                        foreach(var res in comp.Reservations)
                        {
                            if (res.TimeStamp.Day == refDate.Day && res.TimeStamp.Month == refDate.Month && res.TimeStamp.Year == refDate.Year)
                                count++;
                        }
                        dailyRes.Add(count);
                        dailyLabels.Add(refDate.ToShortDateString().ToString());
                        refDate = refDate.AddDays(-1);
                    }

                    List<int> weeklyRes = new List<int>();
                    List<string> weeklyLabels = new List<string>();
                    refDate = DateTime.Now;
                    DateTime startWeek = StartOfWeek(refDate, DayOfWeek.Monday);
                    //DayOfWeek day = refDate.DayOfWeek;
                    //DateTime tillDate = DateTime.Now;
                    for (int i = 0; i < 7; i++)
                    {
                        int count = 0;
                        foreach (var res in comp.Reservations)
                        {
                            if (StartOfWeek(res.TimeStamp, DayOfWeek.Monday) == startWeek)
                                count++;
                        }
                        weeklyRes.Add(count);
                        weeklyLabels.Add(startWeek.ToShortDateString().ToString());
                        startWeek = startWeek.AddDays(-7);
                    }

                    List<int> monthlyRes = new List<int>();
                    List<string> monthlyLabels = new List<string>();
                    refDate = DateTime.Now;
                    for (int i = 0; i < 7; i++)
                    {
                        int count = 0;
                        foreach (var res in comp.Reservations)
                        {
                            if (res.TimeStamp.Month == refDate.Month && res.TimeStamp.Year == refDate.Year)
                                count++;
                        }
                        monthlyRes.Add(count);
                        monthlyLabels.Add(GetMonthName(refDate.Month));
                        refDate = refDate.AddMonths(-1);
                    }
                    model.dailyReservations = dailyRes.Reverse<int>().ToList();
                    model.weeklyReservations = weeklyRes.Reverse<int>().ToList();
                    model.monthlyReservations = monthlyRes.Reverse<int>().ToList();
                    model.dailyLabels = dailyLabels.Reverse<string>().ToList();
                    model.weeklyLabels = weeklyLabels.Reverse<string>().ToList();
                    model.monthlyLabels = monthlyLabels.Reverse<string>().ToList();
                    

                    admin.companyInfo = model;
                    return admin;
                }
            }

            return null;
        }

        private string GetMonthName(int m)
        {
            if (m == 1)
                return "January";
            else if (m == 2)
                return "February";
            else if (m == 3)
                return "March";
            else if (m == 4)
                return "April";
            else if (m == 5)
                return "May";
            else if (m == 6)
                return "June";
            else if (m == 7)
                return "July";
            else if (m == 8)
                return "August";
            else if (m == 9)
                return "September";
            else if (m == 10)
                return "October";
            else if (m == 11)
                return "November";
            else
                return "Decebmer";
        }
        [HttpPost]
        [Route("LoadFlight")]
        public async Task<Object> LoadFlight(IdModel2 idModel)
        {
            FlightListingInfoModel ret = new FlightListingInfoModel();
            List<Airline> companies = new List<Airline>();
            try
            {
                companies = _context2.AirlineCompanies
                .Include(comp => comp.Flights).ThenInclude(f => f.Luggage)
                .Include(fs => fs.Flights).ThenInclude(d => d.From)
                .Include(fs => fs.Flights).ThenInclude(d => d.To)
                .Include(fs => fs.Flights).ThenInclude(d => d.Stops)
                .Include(fs => fs.Flights).ThenInclude(d => d.Raters)
                .Include(fs => fs.Flights).ThenInclude(d => d.SoldTickets)
                .Include(fs => fs.Flights).ThenInclude(d => d.Seats).ThenInclude(d => d.Traveller)
                .Include(fs => fs.Flights).ThenInclude(d => d.SoldTickets)
                .Include(fs => fs.Flights).ThenInclude(d => d.Raters)
                 .Include(comp => comp.Flights).ThenInclude(f => f.Povratnilet)
                .ToList();

            }
            catch (Exception e)
            {

            }
            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            string role = User.Claims.First(c => c.Type == "Roles").Value;


            var user = await _userManager.FindByIdAsync(userid);



            for (int i = 0; i < companies.Count(); i++)
            {
                for (int j = 0; j < companies[i].Flights.Count(); j++)
                {

                    if (companies[i].Flights[j].Id == idModel.Id)
                    {

                        FlightListingInfoModel fi = new FlightListingInfoModel();
                        if (role == "AirlineAdministrator" && user.CompanyId == companies[i].Id)
                            fi.isAdmin = true;
                        else
                            fi.isAdmin = false;

                        if (companies[i].Flights[j].Povratnilet != null)
                        {
                            fi.id = companies[i].Flights[j].Povratnilet.Id;
                            fi.duration = companies[i].Flights[j].Povratnilet.Duration;
                            fi.numOfPassengers = companies[i].Flights[j].Povratnilet.NumOfPassengers;
                            fi.price = companies[i].Flights[j].Povratnilet.Price;
                            fi.departureDate = companies[i].Flights[j].Povratnilet.DepartureDate;
                            fi.idCompany = companies[i].Flights[j].Povratnilet.IdCompany;
                            fi.luggage = new LuggageListingInfoModel() { dimensions = companies[i].Flights[j].Povratnilet.Luggage.Dimensions, quantity = companies[i].Flights[j].Povratnilet.Luggage.Quantity, weight = companies[i].Flights[j].Povratnilet.Luggage.Weight };
                            fi.extra = companies[i].Flights[j].Extra;
                            fi.from = new DestinationListingInfoModel() { id = companies[i].Flights[j].Povratnilet.From.Id, description = companies[i].Flights[j].Povratnilet.From.Description, img = companies[i].Flights[j].Povratnilet.From.Img, name = companies[i].Flights[j].Povratnilet.From.Name };
                            fi.to = new DestinationListingInfoModel() { id = companies[i].Flights[j].Povratnilet.To.Id, description = companies[i].Flights[j].Povratnilet.To.Description, img = companies[i].Flights[j].Povratnilet.To.Img, name = companies[i].Flights[j].Povratnilet.To.Name };
                            fi.airlineId = companies[i].Id;
                            fi.rate = companies[i].Flights[j].Povratnilet.Rate;
                            fi.trip = companies[i].Flights[j].Povratnilet.Trip;
                            if (companies[i].Flights[j].Povratnilet.Stops != null)
                            {
                                foreach (var sh in companies[i].Flights[j].Povratnilet.Stops)
                                {
                                    fi.stops.Add(new DestinationListingInfoModel() { description = sh.Description, id = sh.Id, img = sh.Img, name = sh.Name });
                                }
                            }
                            else
                            {
                                fi.stops = new List<DestinationListingInfoModel>();
                            }
                        }
                        List<DestinationListingInfoModel> pomd = new List<DestinationListingInfoModel>();
                        if (companies[i].Flights[j].Stops != null)
                        {
                            foreach (var sh in companies[i].Flights[j].Stops)
                            {
                                pomd.Add(new DestinationListingInfoModel() { description = sh.Description, id = sh.Id, img = sh.Img, name = sh.Name });
                            }
                        }
                        else
                        {
                            pomd = new List<DestinationListingInfoModel>();
                        }
                        List<SeatListingInfoModel> si = new List<SeatListingInfoModel>();
                        List<DestinationListingInfoModel> di = new List<DestinationListingInfoModel>();
                        List<RaterListingInfoModel> ri = new List<RaterListingInfoModel>();
                        List<TicketListingInfoModel> ti = new List<TicketListingInfoModel>();

                        foreach (var s in companies[i].Flights[j].Seats)
                        {
                            si.Add(new SeatListingInfoModel() { isSelected = s.IsSelected, id = s.Id, taken = s.Taken, traveller = new TravellerInfo(), type = s.Type });
                        }

                        foreach (var s in companies[i].Flights[j].Stops)
                        {
                            di.Add(new DestinationListingInfoModel() { description = s.Description, id = s.Id, img = s.Img, name = s.Name });
                        }
                        foreach (var s in companies[i].Flights[j].Raters)
                        {
                            ri.Add(new RaterListingInfoModel() { id = s.Id, rate = s.Rate });
                        }
                        foreach (var s in companies[i].Flights[j].SoldTickets)
                        {
                            ti.Add(new TicketListingInfoModel()
                            {
                                discount = s.Discount,
                                id = s.Id,
                                flight = new FlightTicket()
                                {
                                    departureDate = s.Flight.DepartureDate,
                                    duration = s.Flight.Duration,
                                    from = new DestinationListingInfoModel()
                                    {
                                        description = s.Flight.From.Description,
                                        name = s.Flight.From.Name,

                                        img = s.Flight.From.Img,
                                        id = s.Flight.From.Id,
                                    },
                                    to = new DestinationListingInfoModel()
                                    {
                                        description = s.Flight.To.Description,
                                        name = s.Flight.To.Name,

                                        img = s.Flight.To.Img,
                                        id = s.Flight.To.Id,
                                    },
                                    id = s.Flight.Id,
                                    idCompany = s.Flight.IdCompany

                                }

                           ,
                                seat = s.Seat
                            });
                        }
                        ret = new FlightListingInfoModel()
                        {
                            seats = si,
                            soldTickets = ti,
                            raters = ri,
                            stops = pomd,
                            rate = companies[i].Flights[j].Rate,
                            trip = companies[i].Flights[j].Trip,
                            airlineId = companies[i].Id,
                            to = new DestinationListingInfoModel()
                            {
                                id = companies[i].Flights[j].To.Id,
                                description = companies[i].Flights[j].To.Description,
                                img = companies[i].Flights[j].To.Img,
                                name = companies[i].Flights[j].To.Name
                            },
                            from = new DestinationListingInfoModel()
                            {
                                id = companies[i].Flights[j].From.Id,
                                description = companies[i].Flights[j].From.Description,
                                img = companies[i].Flights[j].From.Img,
                                name = companies[i].Flights[j].From.Name
                            },
                            extra = companies[i].Flights[j].Extra,
                            luggage = new LuggageListingInfoModel()
                            {
                                dimensions = companies[i].Flights[j].Luggage.Dimensions,
                                quantity = companies[i].Flights[j].Luggage.Quantity,
                                weight = companies[i].Flights[j].Luggage.Weight
                            },
                            price = companies[i].Flights[j].Price,
                            id = companies[i].Flights[j].Id,
                            duration = companies[i].Flights[j].Duration,
                            numOfPassengers = companies[i].Flights[j].NumOfPassengers,
                            departureDate = companies[i].Flights[j].DepartureDate,
                            idCompany = companies[i].Flights[j].IdCompany,
                            povratniLet = fi,

                        };
                    }
                }
            }
            return ret;
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("ChangeAdminAccountDetails")]
        public async Task<object> ChangeAdminAccountDetails(AdminDetailsModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);
            if( await _userManager.CheckPasswordAsync(user, model.password))
            {
                user.FullName = model.name;
                user.Address = model.addr;
                if (user.Email != model.email)
                {
                    user.EmailConfirmed = false;
                    string toMail = "http://localhost:57886/api/AppUser/VerifyEmail/" + user.Id;

                    MailMessage mail = new MailMessage();
                    mail.To.Add(model.email);
                    mail.From = new MailAddress("web2020tim1718@gmail.com");
                    mail.Subject = "Projekat";
                    mail.Body = "Please verify your e-mail address by clicking this link: ";
                    mail.Body += toMail;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new System.Net.NetworkCredential("web2020tim1718@gmail.com", "NinaStanka987");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                user.Email = model.email;
                user.Birthday = DateTime.Parse(model.bd);
                await _userManager.UpdateAsync(user);
                _context2.Update(user);
                _context2.SaveChanges();

                user = await _userManager.FindByIdAsync(userId);

                CarRentalAdminModel admin = new CarRentalAdminModel();
                admin.address = user.Address;
                admin.birthday = user.Birthday.ToShortDateString();
                admin.companyId = user.CompanyId;
                admin.email = user.Email;
                admin.fullName = user.FullName;
                admin.username = user.UserName;
                admin.verifiedEmail = true;
                admin.changedPassword = user.ChangedPassword;

                return admin;
            }
            

            return BadRequest(new { message = "Wrong password." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("ChangeAdminUsername")]
        public async Task<object> ChangeAdminUsername(AdminUsernameModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);
            

            if (await _userManager.CheckPasswordAsync(user, model.password))
            {
                var users = _context2.Users.ToList();
                foreach(var us in users)
                {
                    if(us.UserName == model.username)
                    {
                        return BadRequest(new { message = "Username already exists." });
                    }
                }
                user.UserName = model.username;
                await _userManager.UpdateAsync(user);
                _context2.Update(user);
                _context2.SaveChanges();

                user = await _userManager.FindByIdAsync(userId);

                CarRentalAdminModel admin = new CarRentalAdminModel();
                admin.address = user.Address;
                admin.birthday = user.Birthday.ToShortDateString();
                admin.companyId = user.CompanyId;
                admin.email = user.Email;
                admin.fullName = user.FullName;
                admin.username = user.UserName;
                admin.verifiedEmail = true;
                admin.changedPassword = user.ChangedPassword;

                return admin;
            }


            return BadRequest(new { message = "Wrong password." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("ChangeAdminPassword")]
        public async Task<object> ChangeAdminPassword(AdminPasswordModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);


            if (await _userManager.CheckPasswordAsync(user, model.password))
            {
                var res = await _userManager.ChangePasswordAsync(user, model.password, model.newPassword);
                user.ChangedPassword = true;
                await _userManager.UpdateAsync(user);
                _context2.Update(user);
                _context2.SaveChanges();

                user = await _userManager.FindByIdAsync(userId);

                CarRentalAdminModel admin = new CarRentalAdminModel();
                admin.address = user.Address;
                admin.birthday = user.Birthday.ToShortDateString();
                admin.companyId = user.CompanyId;
                admin.email = user.Email;
                admin.fullName = user.FullName;
                admin.username = user.UserName;
                admin.verifiedEmail = true;
                admin.changedPassword = user.ChangedPassword;

                return admin;
            }


            return BadRequest(new { message = "Wrong password." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("UpdateCompanyInfo")]
        public async Task<object> UpdateCompanyInfo(UpdateCompanyInfoModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);

            if(user.CompanyId != model.compId)
            {
                return BadRequest(new { message = "Not your company." });
            }

            var options = new DbContextOptionsBuilder<ApplicationUsersContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            using (var context = new ApplicationUsersContext(options))
            {
                var companies = context.RentACarCompanies.ToList();

                foreach (var comp in companies)
                {
                    if (comp.ID == model.compId)
                    {
                        comp.Name = model.name;
                        comp.Description = model.description;
                        comp.Image = model.logo;
                        comp.Activated = true;
                        context.Update(comp);
                        try
                        {
                            await context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            return BadRequest(new { message = "Car has been changed. Please reload the page." });
                        }
                        
                        return Ok();
                    }
                }
            }

            return BadRequest(new { message = "Company not found." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("AddCarCompanyLocation")]
        public async Task<object> AddCarCompanyLocation(NewLocationModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user.CompanyId != model.compId)
            {
                return BadRequest(new { message = "Not your company." });
            }

            var companies = _context2.RentACarCompanies
                .Include(comp=>comp.MainLocation)
                .Include(comp=>comp.Locations)
                .ToList();

            foreach (var comp in companies)
            {
                if (comp.ID == model.compId)
                {
                    Address newLoc = new Address();
                    newLoc.FullAddress = model.address;
                    newLoc.Latitude = model.latitude;
                    newLoc.Longitude = model.longitude;

                    if(comp.Locations == null)
                    {
                        comp.Locations = new List<Address>();
                    }
                    comp.Locations.Add(newLoc);
                    _context2.Update(comp);
                    _context2.SaveChanges();
                    return comp.Locations;
                }
            }
            return BadRequest(new { message = "Company not found." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("EditCarCompanyLocation")]
        public async Task<object> EditCarCompanyLocation(EditLocationModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user.CompanyId != model.compId)
            {
                return BadRequest(new { message = "Not your company." });
            }

            var companies = _context2.RentACarCompanies
                .Include(comp => comp.MainLocation)
                .Include(comp => comp.Locations)
                .Include(comp => comp.Reservations)
                .Include(comp => comp.Cars)
                .ToList();

            var company = await _context2.RentACarCompanies.FindAsync(model.compId);

            if(company == null)
                return BadRequest(new { message = "Company not found." });

            Address addr = _context2.RentACarCompanies.Find(model.compId).Locations.Find(loc => loc.Id == model.locId);
            if(addr == null)
                return BadRequest(new { message = "Address not found." });

            foreach(var res in company.Reservations)
            {
                if(res.PickUpLocation.Trim().ToLower().Contains(model.address.ToLower().Trim()) || res.ReturnLocation.Trim().ToLower().Contains(model.address.ToLower().Trim()))
                    return BadRequest(new { message = "There is a reservation made using this address, can not be edited" });
            }

            foreach(var car in company.Cars)
            {
                if (car.Location == addr.FullAddress)
                    car.Location = model.address;
            }

            addr.Latitude = model.latitude;
            addr.Longitude = model.longitude;
            addr.FullAddress = model.address;

            _context2.Update(company);
            try
            {
                await _context2.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(new { message = "Data has been modified. Please reload the page." });
            }

            return company.Locations;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("RemoveCarCompanyLocation")]
        public async Task<object> RemoveCarCompanyLocation(RemoveLocationModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user.CompanyId != model.id2)
            {
                return BadRequest(new { message = "Not your company." });
            }

            var companies = _context2.RentACarCompanies
                .Include(comp => comp.MainLocation)
                .Include(comp => comp.Locations)
                .Include(comp => comp.Reservations)
                .Include(comp => comp.Cars)
                .ToList();

            var company = await _context2.RentACarCompanies.FindAsync(model.id2);

            if (company == null)
                return BadRequest(new { message = "Company not found." });

            Address addr = _context2.RentACarCompanies.Find(model.id2).Locations.Find(loc => loc.Id == model.id);
            if (addr == null)
                return BadRequest(new { message = "Address not found." });

            foreach (var res in company.Reservations)
            {
                if (res.PickUpLocation.Trim().ToLower().Contains(addr.FullAddress.ToLower().Trim()) || res.ReturnLocation.Trim().ToLower().Contains(addr.FullAddress.ToLower().Trim()))
                    return BadRequest(new { message = "Reservation was made using this address, can not remove." });
            }

            foreach (var car in company.Cars)
            {
                if (car.Location == addr.FullAddress)
                    car.Location = model.newAddr;
            }

            company.Locations.Remove(addr);

            _context2.Update(company);
            try
            {
                await _context2.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(new { message = "Data has been modified. Please reload the page." });
            }

            return company.Locations;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("UpdateCar")]
        public async Task<object> UpdateCar(UpdateCarModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user.CompanyId != model.companyId)
            {
                return BadRequest(new { message = "Not your company." });
            }

            var options = new DbContextOptionsBuilder<ApplicationUsersContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            using (var context = new ApplicationUsersContext(options))
            {
                var companies = context.RentACarCompanies
                   .Include(comp => comp.MainLocation)
                   .Include(comp => comp.Locations)
                   .Include(comp => comp.Cars).ThenInclude(car => car.RentedDates)
                   .ToList();
                var comp = context.RentACarCompanies.Find(model.companyId);
                if(comp == null)
                    return BadRequest(new { message = "Company not found." });
                var carr = context.RentACarCompanies.Find(model.companyId).Cars.Find(car => car.ID == model.carId);
                if(carr == null || carr.Removed)
                    return BadRequest(new { message = "Car is already removed." });

                foreach (var d in carr.RentedDates)
                {
                    DateTime pom = DateTime.Parse(d.DateStr);
                    if (pom > DateTime.Now)
                        return BadRequest(new { message = "Not able to edit car that is rented." });
                }

                carr.Brand = model.brand;
                carr.Location = model.loc;
                carr.Model = model.model;
                carr.Passengers = model.passen;
                carr.PricePerDay = model.price;
                carr.Type = model.type;
                carr.Year = model.year;

                context.Update(carr);

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return BadRequest(new { message = "Car has been modified, please reload the page." });
                }

                List<CarModelAdmin> cars = new List<CarModelAdmin>();
                foreach (var car in comp.Cars)
                {
                    if (!car.Removed)
                    {
                        List<string> rented = new List<string>();
                        foreach (var date in car.RentedDates)
                        {
                            rented.Add(date.DateStr);
                        }
                        cars.Add(new CarModelAdmin() { avrageRating = car.AvrageRating, brand = car.Brand, id = car.ID, image = car.Image, location = car.Location, model = car.Model, passengers = car.Passengers, price = car.PricePerDay, type = car.Type, year = car.Year, rentedDates = rented });
                    }
                    
                }

                return cars;
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("RemoveCar")]
        public async Task<object> RemoveCar(HelpModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);

            var options = new DbContextOptionsBuilder<ApplicationUsersContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            using(var context = new ApplicationUsersContext(options))
            {
                var companies = context.RentACarCompanies
                .Include(comp => comp.MainLocation)
                .Include(comp => comp.Locations)
                .Include(comp => comp.Cars).ThenInclude(car => car.RentedDates)
                .Include(comp => comp.QuickReservations).ThenInclude(res => res.Dates)
                .ToList();

                var comp = context.RentACarCompanies.Find(user.CompanyId);
                if(comp == null)
                    return BadRequest(new { message = "Company not found." });
                var carr = context.RentACarCompanies.Find(user.CompanyId).Cars.Find(car => car.ID == model.id);
                if(carr == null)
                    return BadRequest(new { message = "Car not found." });
                if(!carr.Removed)
                {
                    foreach (var d in carr.RentedDates)
                    {
                        DateTime pom = DateTime.Parse(d.DateStr);
                        if (pom > DateTime.Now)
                            return BadRequest(new { message = "Not able to remove car that is rented." });
                    }
                    carr.Removed = true;
                    context.Update(carr);
                    await context.SaveChangesAsync();
                }
                

                List<CarModelAdmin> cars = new List<CarModelAdmin>();
                foreach (var car in comp.Cars)
                {
                    if (!car.Removed)
                    {
                        List<string> rented = new List<string>();
                        foreach (var date in car.RentedDates)
                        {
                            rented.Add(date.DateStr);
                        }
                        cars.Add(new CarModelAdmin() { avrageRating = car.AvrageRating, brand = car.Brand, id = car.ID, image = car.Image, location = car.Location, model = car.Model, passengers = car.Passengers, price = car.PricePerDay, type = car.Type, year = car.Year, rentedDates = rented });
                    }
                    
                }

                return cars;


            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("AddNewCar")]
        public async Task<object> AddNewCar(NewCarModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user.CompanyId != model.companyId)
            {
                return BadRequest(new { message = "Not your company." });
            }

            var companies = _context2.RentACarCompanies
                .Include(comp => comp.MainLocation)
                .Include(comp => comp.Locations)
                .Include(comp => comp.Cars).ThenInclude(car => car.RentedDates)
                .ToList();

            foreach (var comp in companies)
            {
                if (comp.ID == model.companyId)
                {
                    Car newCar = new Car();
                    newCar.AvrageRating = 0;
                    newCar.Brand = model.brand;
                    newCar.CompanyId = model.companyId;
                    newCar.Image = model.image;
                    newCar.Location = model.loc;
                    newCar.Model = model.model;
                    newCar.Passengers = model.passen;
                    newCar.PricePerDay = model.price;
                    newCar.Ratings = new List<Rating>();
                    newCar.RentedDates = new List<Date>();
                    newCar.Type = model.type;
                    newCar.Year = model.year;

                    comp.Cars.Add(newCar);
                    _context2.Update(comp);
                    await _context2.SaveChangesAsync();

                    List<CarModelAdmin> cars = new List<CarModelAdmin>();
                    foreach (var car in comp.Cars)
                    {
                        if (!car.Removed)
                        {
                            List<string> rented = new List<string>();
                            foreach (var date in car.RentedDates)
                            {
                                rented.Add(date.DateStr);
                            }
                            cars.Add(new CarModelAdmin() { avrageRating = car.AvrageRating, brand = car.Brand, id = car.ID, image = car.Image, location = car.Location, model = car.Model, passengers = car.Passengers, price = car.PricePerDay, type = car.Type, year = car.Year, rentedDates = rented });
                        }
                        
                    }

                    return cars;

                    
                }
            }
            return BadRequest(new { message = "Company not found." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("UpdateAmenity")]
        public async Task<object> UpdateAmenity(UpdateAmenityModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return BadRequest(new { message = "Forbbiden action for this role." });
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user.CompanyId != model.companyId)
            {
                return BadRequest(new { message = "Not your company." });
            }

            var options = new DbContextOptionsBuilder<ApplicationUsersContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            using(var context = new ApplicationUsersContext(options))
            {
                var companies = context.RentACarCompanies
                .Include(comp => comp.extras)
                .Include(comp=>comp.Reservations).ThenInclude(res=>res.Extras)
                .ToList();

                var company = await context.RentACarCompanies.FindAsync(model.companyId);
                var am = context.RentACarCompanies.Find(model.companyId).extras.Find(am => am.Id == model.amenityId);

                foreach(var res in company.Reservations)
                {
                    if(res.Extras.Find(ame=>ame.Id == am.Id) != null)
                    {
                        return BadRequest(new { message = "Amenity is in reservation" });
                    }
                }

                am.Name = model.name;
                am.Price = model.price;
                am.Image = model.image;
                context.Update(am);
                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return BadRequest(new { message = "Data had already been modified, please reload the page." });
                }
                return company.extras;
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("RemoveAmenity")]
        public async Task<object> RemoveAmenity(IdModel3 model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return BadRequest(new { message = "Forbbiden action for this role." });
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user.CompanyId != model.id2)
            {
                return BadRequest(new { message = "Not your company." });
            }

            var options = new DbContextOptionsBuilder<ApplicationUsersContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            using (var context = new ApplicationUsersContext(options))
            {
                var companies = context.RentACarCompanies
                .Include(comp => comp.extras)
                .Include(comp => comp.Reservations).ThenInclude(res => res.Extras)
                .ToList();

                var company = await context.RentACarCompanies.FindAsync(model.id2);
                var am = context.RentACarCompanies.Find(model.id2).extras.Find(am => am.Id == model.id);

                foreach (var res in company.Reservations)
                {
                    if (res.Extras.Find(ame => ame.Id == am.Id) != null)
                    {
                        return BadRequest(new { message = "Amenity is in reservation" });
                    }
                }

                company.extras.Remove(am);
                context.Update(company);
                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return BadRequest(new { message = "Data had already been modified, please reload the page." });
                }
                return company.extras;
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("AddAmenity")]
        public async Task<object> AddAmenity(AddAmenityModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return BadRequest(new { message = "Forbbiden action for this role." });
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user.CompanyId != model.companyId)
            {
                return BadRequest(new { message = "Not your company." });
            }

            var companies = _context2.RentACarCompanies
                .Include(comp => comp.extras)
                .ToList();

            foreach (var comp in companies)
            {
                if (model.companyId == comp.ID)
                {
                    ExtraAmenity amenity = new ExtraAmenity();
                    amenity.Image = model.image;
                    amenity.Name = model.name;
                    amenity.Price = model.price;
                    if(model.payment == "One Time Payment")
                    {
                        amenity.OneTimePayment = true;
                    }
                    else
                    {
                        amenity.OneTimePayment = false;
                    }
                    comp.extras.Add(amenity);
                    _context2.Update(comp);
                    _context2.SaveChanges();
                    return comp.extras;
                }
            }

            return BadRequest(new { message = "Something went wrong, please reload and try again." });
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("LoadWebsiteAdmin")]
        public async Task<object> LoadWebsiteAdmin()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "WebsiteAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);
            var allUsers = _context2.Users.Include(us => us.Discount).ToList();

            

            WebsiteAdminModel admin = new WebsiteAdminModel();
            admin.address = user.Address;
            admin.birthday = user.Birthday.ToShortDateString();
            admin.email = user.Email;
            admin.fullName = user.FullName;
            admin.username = user.UserName;
            admin.verifiedEmail = true;
            admin.changedPassword = user.ChangedPassword;
            admin.mainAdmin = user.MainWebsiteAdministrator;
            admin.websiteAdministrators = new List<OtherAdmin>();
            foreach (var u in allUsers)
            {
                if (u.Id == userId)
                {
                    admin.discount = new DiscountModel() { bronzeTier = u.Discount.BronzeTier, discountPercent = u.Discount.DiscountPercent, goldTier = u.Discount.GoldTier, silverTier = u.Discount.SilverTier };
                }
            }
            
            var users = await _userManager.GetUsersInRoleAsync("WebsiteAdministrator");

            foreach (var u in users)
            {
                if(!u.MainWebsiteAdministrator)
                {
                    admin.websiteAdministrators.Add(new OtherAdmin() { email = u.Email, fullname = u.FullName, username = u.UserName });
                }
            }

            var companies = _context2.RentACarCompanies.ToList();
            var companies2 = _context2.AirlineCompanies.ToList();
            var users2 = await _userManager.GetUsersInRoleAsync("RentACarAdministrator");
            var users3 = await _userManager.GetUsersInRoleAsync("AirlineAdministrator");

            List<CarCompanyModel> carCompanies = new List<CarCompanyModel>();

            foreach(var comp in companies)
            {
                CarCompanyModel com = new CarCompanyModel();
                com.name = comp.Name;
                com.image = comp.Image;
                com.id = comp.ID;
                List<string> carAdmins = new List<string>();
                foreach(var u in users2)
                {
                    if(u.CompanyId == comp.ID)
                    {
                        carAdmins.Add(u.UserName);
                    }
                }
                com.admins = carAdmins;
                carCompanies.Add(com);
            }
            admin.rentAcars = carCompanies;

            List<AirCompanyModel> airCompanies = new List<AirCompanyModel>();

            foreach (var comp in companies2)
            {
                AirCompanyModel com = new AirCompanyModel();
                com.name = comp.Name;
                com.image = comp.Img;
                com.id = comp.Id;
                List<string> airAdmins = new List<string>();
                foreach (var u in users3)
                {
                    if (u.CompanyId == comp.Id)
                    {
                        airAdmins.Add(u.UserName);
                    }
                }
                com.admins = airAdmins;
                airCompanies.Add(com);
            }
            admin.airlines = airCompanies;

            return admin;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("ChangeWAdminAccountDetails")]
        public async Task<object> ChangeWAdminAccountDetails(AdminDetailsModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "WebsiteAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (await _userManager.CheckPasswordAsync(user, model.password))
            {
                user.FullName = model.name;
                user.Address = model.addr;
                if (user.Email != model.email)
                {
                    user.EmailConfirmed = false;
                    string toMail = "http://localhost:57886/api/AppUser/VerifyEmail/" + user.Id;

                    MailMessage mail = new MailMessage();
                    mail.To.Add(model.email);
                    mail.From = new MailAddress("web2020tim1718@gmail.com");
                    mail.Subject = "Projekat";
                    mail.Body = "Please verify your e-mail address by clicking this link: ";
                    mail.Body += toMail;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new System.Net.NetworkCredential("web2020tim1718@gmail.com", "NinaStanka987");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                   
                user.Email = model.email;
                user.Birthday = DateTime.Parse(model.bd);
                await _userManager.UpdateAsync(user);
                _context2.Update(user);
                _context2.SaveChanges();

                user = await _userManager.FindByIdAsync(userId);

                WebsiteAdminModel admin = new WebsiteAdminModel();
                admin.address = user.Address;
                admin.birthday = user.Birthday.ToShortDateString();
                admin.email = user.Email;
                admin.fullName = user.FullName;
                admin.username = user.UserName;
                admin.verifiedEmail = true;
                admin.changedPassword = user.ChangedPassword;
                admin.mainAdmin = user.MainWebsiteAdministrator;

                return admin;
            }


            return BadRequest(new { message = "Wrong password." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("ChangeWAdminUsername")]
        public async Task<object> ChangeWAdminUsername(AdminUsernameModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "WebsiteAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);


            if (await _userManager.CheckPasswordAsync(user, model.password))
            {
                var users = _context2.Users.ToList();
                foreach (var us in users)
                {
                    if (us.UserName == model.username)
                    {
                        return BadRequest(new { message = "Username already exists." });
                    }
                }
                user.UserName = model.username;
                await _userManager.UpdateAsync(user);
                _context2.Update(user);
                _context2.SaveChanges();

                user = await _userManager.FindByIdAsync(userId);

                WebsiteAdminModel admin = new WebsiteAdminModel();
                admin.address = user.Address;
                admin.birthday = user.Birthday.ToShortDateString();
                admin.email = user.Email;
                admin.fullName = user.FullName;
                admin.username = user.UserName;
                admin.verifiedEmail = true;
                admin.changedPassword = user.ChangedPassword;
                admin.mainAdmin = user.MainWebsiteAdministrator;

                return admin;
            }


            return BadRequest(new { message = "Wrong password." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("ChangeWAdminPassword")]
        public async Task<object> ChangeWAdminPassword(AdminPasswordModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "WebsiteAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (await _userManager.CheckPasswordAsync(user, model.password))
            {
                var res = await _userManager.ChangePasswordAsync(user, model.password, model.newPassword);
                user.ChangedPassword = true;
                await _userManager.UpdateAsync(user);
                _context2.Update(user);
                _context2.SaveChanges();

                user = await _userManager.FindByIdAsync(userId);

                WebsiteAdminModel admin = new WebsiteAdminModel();
                admin.address = user.Address;
                admin.birthday = user.Birthday.ToShortDateString();
                admin.email = user.Email;
                admin.fullName = user.FullName;
                admin.username = user.UserName;
                admin.verifiedEmail = true;
                admin.changedPassword = user.ChangedPassword;
                admin.mainAdmin = user.MainWebsiteAdministrator;
                return admin;
            }
            return BadRequest(new { message = "Wrong password." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("AddNewWebAdmin")]
        public async Task<object> AddNewWebAdmin(NewWebAdminModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "WebsiteAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);
            if(user.MainWebsiteAdministrator)
            {
                if (_userManager.FindByNameAsync(model.username).Result == null)
                {
                    User newAdmin = new User();
                    newAdmin.UserName = model.username;
                    newAdmin.Email = model.email;
                    newAdmin.FullName = "";
                    newAdmin.Birthday = new DateTime(2000, 1, 1);
                    newAdmin.Address = "";
                    newAdmin.PhoneNumber = "";
                    newAdmin.ChangedPassword = false;
                    newAdmin.MainWebsiteAdministrator = false;

                    var users = _context2.Users.Include(us => us.Discount).ToList();

                    foreach (var u in users)
                    {
                        if (u.Id == userId)
                        {
                            newAdmin.Discount = u.Discount;
                        }
                    }

                    IdentityResult result = _userManager.CreateAsync(newAdmin, model.password).Result;

                    if (result.Succeeded)
                    {
                        try
                        {
                            _userManager.AddToRoleAsync(newAdmin, "WebsiteAdministrator").Wait();

                            string toMail = "http://localhost:57886/api/AppUser/VerifyEmail/" + newAdmin.Id;

                            MailMessage mail = new MailMessage();
                            mail.To.Add(newAdmin.Email);
                            mail.From = new MailAddress("web2020tim1718@gmail.com");
                            mail.Subject = "Projekat";
                            mail.Body = "Dear new Administrator, your username is " + newAdmin.UserName + " and your password is: " + model.password + ". To activate your account you must change your password. Please verify your e-mail address by clicking this link: ";
                            mail.Body += toMail;

                            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                            {
                                smtp.Credentials = new System.Net.NetworkCredential("web2020tim1718@gmail.com", "NinaStanka987");
                                smtp.EnableSsl = true;
                                smtp.Send(mail);
                            }


                            List<OtherAdmin> admins = new List<OtherAdmin>();

                            var users2 = await _userManager.GetUsersInRoleAsync("WebsiteAdministrator");

                            foreach (var u in users2)
                            {
                                if (!u.MainWebsiteAdministrator)
                                {
                                    admins.Add(new OtherAdmin() { email = u.Email, fullname = u.FullName, username = u.UserName });
                                }
                            }
                            return admins;
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
                else
                {
                    return BadRequest(new { message = "Username already exists." });
                }
                

                user = await _userManager.FindByIdAsync(userId);

                WebsiteAdminModel admin = new WebsiteAdminModel();
                admin.address = user.Address;
                admin.birthday = user.Birthday.ToShortDateString();
                admin.email = user.Email;
                admin.fullName = user.FullName;
                admin.username = user.UserName;
                admin.verifiedEmail = true;
                admin.changedPassword = user.ChangedPassword;
                admin.mainAdmin = user.MainWebsiteAdministrator;
                return admin;
            }
            else
                return BadRequest(new { message = "Not a main administrator." });

        }


        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("RegisterCarCompany")]
        public async Task<object> RegisterCarCompany(RegisterCompanyModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "WebsiteAdministrator")
            {
                return "Forbbiden action for this role.";
            }

            if (_userManager.FindByNameAsync(model.username).Result == null)
            {
                Address addr1 = new Address() { FullAddress = model.address, Latitude = model.latitude, Longitude = model.longitude };
                RentACar company1 = new RentACar();
                company1.Activated = false;
                company1.AvrageRating = 0;
                company1.Cars = new List<Car>();
                company1.Description = "";
                company1.extras = new List<ExtraAmenity>();
                company1.Image = "";
                company1.Locations = new List<Address>();
                company1.MainLocation = addr1;
                company1.Name = model.companyName;
                company1.QuickReservations = new List<QuickReservation>();
                company1.Ratings = new List<Rating>();
                company1.extras = new List<ExtraAmenity>();

                _context2.RentACarCompanies.Add(company1);
                _context2.SaveChanges();

                int id = company1.ID;

                User newAdmin = new User();
                newAdmin.UserName = model.username;
                newAdmin.Email = model.email;
                newAdmin.FullName = "";
                newAdmin.Birthday = new DateTime(2000, 1, 1);
                newAdmin.Address = "";
                newAdmin.PhoneNumber = "";
                newAdmin.ChangedPassword = false;
                newAdmin.CompanyId = id;

                IdentityResult result = _userManager.CreateAsync(newAdmin, model.password).Result;

                if (result.Succeeded)
                {
                    try
                    {
                        _userManager.AddToRoleAsync(newAdmin, "RentACarAdministrator").Wait();
                        List<OtherAdmin> admins = new List<OtherAdmin>();

                        var users = await _userManager.GetUsersInRoleAsync("RentACarAdministrator");
                        var companies = _context2.RentACarCompanies.ToList();

                        List<CarCompanyModel> carCompanies = new List<CarCompanyModel>();

                        foreach (var comp in companies)
                        {
                            CarCompanyModel com = new CarCompanyModel();
                            com.name = comp.Name;
                            com.image = comp.Image;
                            com.id = comp.ID;
                            List<string> carAdmins = new List<string>();
                            foreach (var u in users)
                            {
                                if (u.CompanyId == comp.ID)
                                {
                                    carAdmins.Add(u.UserName);
                                }
                            }
                            com.admins = carAdmins;
                            carCompanies.Add(com);
                        }
                        return carCompanies;
                    }
                    catch (Exception e)
                    {

                    }
                }
                else
                {
                    return BadRequest(new { message = "Error creating user" });
                }
            }
            else
            {
                return BadRequest(new { message = "Username already exists." });
            }
            return BadRequest(new { message = "Error." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("AddAdminToCompany")]
        public async Task<object> AddAdminToCompany(AddAdminModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "WebsiteAdministrator")
            {
                return "Forbbiden action for this role.";
            }

            if (_userManager.FindByNameAsync(model.username).Result == null)
            {
                var companies = _context2.RentACarCompanies.ToList();
                bool exists = false;
                foreach(var comp in companies)
                {
                    if(comp.ID == model.companyId)
                    {
                        exists = true;
                    }
                }

                if(!exists)
                {
                    return "Company no longer exists.";
                }

                User newAdmin = new User();
                newAdmin.UserName = model.username;
                newAdmin.Email = model.email;
                newAdmin.FullName = "";
                newAdmin.Birthday = new DateTime(2000, 1, 1);
                newAdmin.Address = "";
                newAdmin.PhoneNumber = "";
                newAdmin.ChangedPassword = false;
                newAdmin.CompanyId = model.companyId;

                IdentityResult result = _userManager.CreateAsync(newAdmin, model.password).Result;

                if (result.Succeeded)
                {
                    try
                    {
                        _userManager.AddToRoleAsync(newAdmin, "RentACarAdministrator").Wait();

                        string toMail = "http://localhost:57886/api/AppUser/VerifyEmail/" + newAdmin.Id;

                        MailMessage mail = new MailMessage();
                        mail.To.Add(newAdmin.Email);
                        mail.From = new MailAddress("web2020tim1718@gmail.com");
                        mail.Subject = "Projekat";
                        mail.Body = "Dear car rental administrator, your username is " + newAdmin.UserName + " and your password is: " + model.password + ". To activate your account you must change your password. Please verify your e-mail address by clicking this link: ";
                        mail.Body += toMail;

                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.Credentials = new System.Net.NetworkCredential("web2020tim1718@gmail.com", "NinaStanka987");
                            smtp.EnableSsl = true;
                            smtp.Send(mail);
                        }

                        List<OtherAdmin> admins = new List<OtherAdmin>();

                        var users = await _userManager.GetUsersInRoleAsync("RentACarAdministrator");
                        

                        List<CarCompanyModel> carCompanies = new List<CarCompanyModel>();

                        foreach (var comp in companies)
                        {
                            CarCompanyModel com = new CarCompanyModel();
                            com.name = comp.Name;
                            com.image = comp.Image;
                            com.id = comp.ID;
                            List<string> carAdmins = new List<string>();
                            foreach (var u in users)
                            {
                                if (u.CompanyId == comp.ID)
                                {
                                    carAdmins.Add(u.UserName);
                                }
                            }
                            com.admins = carAdmins;
                            carCompanies.Add(com);
                        }
                        return carCompanies;
                    }
                    catch (Exception e)
                    {

                    }
                }
                else
                {
                    return BadRequest(new { message = "Error creating user" });
                }
            }
            else
            {
                return BadRequest(new { message = "Username already exists." });
            }
            return BadRequest(new { message = "Error." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("AddAdminToAirCompany")]
        public async Task<object> AddAdminToAirCompany(AddAdminModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "WebsiteAdministrator")
            {
                return "Forbbiden action for this role.";
            }

            if (_userManager.FindByNameAsync(model.username).Result == null)
            {
                var companies = _context2.AirlineCompanies.ToList();
                bool exists = false;
                foreach (var comp in companies)
                {
                    if (comp.Id == model.companyId)
                    {
                        exists = true;
                    }
                }

                if (!exists)
                {
                    return "Company no longer exists.";
                }

                User newAdmin = new User();
                newAdmin.UserName = model.username;
                newAdmin.Email = model.email;
                newAdmin.FullName = "";
                newAdmin.Birthday = new DateTime(2000, 1, 1);
                newAdmin.Address = "";
                newAdmin.PhoneNumber = "";
                newAdmin.ChangedPassword = false;
                newAdmin.CompanyId = model.companyId;

                IdentityResult result = _userManager.CreateAsync(newAdmin, model.password).Result;

                if (result.Succeeded)
                {
                    try
                    {
                        _userManager.AddToRoleAsync(newAdmin, "AirlineAdministrator").Wait();

                        string toMail = "http://localhost:57886/api/AppUser/VerifyEmail/" + newAdmin.Id;

                        MailMessage mail = new MailMessage();
                        mail.To.Add(newAdmin.Email);
                        mail.From = new MailAddress("web2020tim1718@gmail.com");
                        mail.Subject = "Projekat";
                        mail.Body = "Dear new airline administrator, your username is " + newAdmin.UserName + " and your password is: " + model.password + ". To activate your account you must change your password. Please verify your e-mail address by clicking this link: ";
                        mail.Body += toMail;

                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.Credentials = new System.Net.NetworkCredential("web2020tim1718@gmail.com", "NinaStanka987");
                            smtp.EnableSsl = true;
                            smtp.Send(mail);
                        }

                        List<OtherAdmin> admins = new List<OtherAdmin>();

                        var users = await _userManager.GetUsersInRoleAsync("AirlineAdministrator");


                        List<AirCompanyModel> airCompanies = new List<AirCompanyModel>();

                        foreach (var comp in companies)
                        {
                            AirCompanyModel com = new AirCompanyModel();
                            com.name = comp.Name;
                            com.image = comp.Img;
                            com.id = comp.Id;
                            List<string> airAdmins = new List<string>();
                            foreach (var u in users)
                            {
                                if (u.CompanyId == comp.Id)
                                {
                                    airAdmins.Add(u.UserName);
                                }
                            }
                            com.admins = airAdmins;
                            airCompanies.Add(com);
                        }
                        return airCompanies;
                    }
                    catch (Exception e)
                    {

                    }
                }
                else
                {
                    return BadRequest(new { message = "Error creating user" });
                }
            }
            else
            {
                return BadRequest(new { message = "Username already exists." });
            }
            return BadRequest(new { message = "Error." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("RegisterAirCompany")]
        public async Task<object> RegisterAirCompany(RegisterAirCompanyModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "WebsiteAdministrator")
            {
                return "Forbbiden action for this role.";
            }

            if (_userManager.FindByNameAsync(model.username).Result == null)
            {
                Airline airline = new Airline();
                airline.Name = model.companyName;
                airline.FastTickets = new List<Ticket>();
                airline.Raters = new List<Rater>();
                airline.Address = model.address;
                airline.Description = "";
                airline.Destinations = new List<Destination>();
                airline.Img = "";
                airline.Rating = 0;

                _context2.AirlineCompanies.Add(airline);
                _context2.SaveChanges();

                User newAdmin = new User();
                newAdmin.UserName = model.username;
                newAdmin.Email = model.email;
                newAdmin.FullName = "";
                newAdmin.Birthday = new DateTime(2000, 1, 1);
                newAdmin.Address = "";
                newAdmin.PhoneNumber = "";
                newAdmin.ChangedPassword = false;
                newAdmin.CompanyId = airline.Id;

                IdentityResult result = _userManager.CreateAsync(newAdmin, model.password).Result;

                if (result.Succeeded)
                {
                    try
                    {
                        _userManager.AddToRoleAsync(newAdmin, "AirlineAdministrator").Wait();
                        List<OtherAdmin> admins = new List<OtherAdmin>();

                        var users = await _userManager.GetUsersInRoleAsync("AirlineAdministrator");
                        var companies = _context2.AirlineCompanies.ToList();

                        List<AirCompanyModel> airCompanies = new List<AirCompanyModel>();

                        foreach (var comp in companies)
                        {
                            AirCompanyModel com = new AirCompanyModel();
                            com.name = comp.Name;
                            com.image = comp.Img;
                            com.id = comp.Id;
                            List<string> airAdmins = new List<string>();
                            foreach (var u in users)
                            {
                                if (u.CompanyId == comp.Id)
                                {
                                    airAdmins.Add(u.UserName);
                                }
                            }
                            com.admins = airAdmins;
                            airCompanies.Add(com);
                        }
                        return airCompanies;
                    }
                    catch (Exception e)
                    {

                    }
                }
                else
                {
                    return BadRequest(new { message = "Error creating user" });
                }
            }
            else
            {
                return BadRequest(new { message = "Username already exists." });
            }
            return BadRequest(new { message = "Error." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("SaveDiscount")]
        public async Task<object> SaveDiscount(DiscountModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "WebsiteAdministrator")
            {
                return BadRequest(new { message = "Forbbiden action for this role." });
            }

            var u = await _userManager.FindByIdAsync(userId);
            if(u == null)
                BadRequest(new { message = "User not found." });

            var options = new DbContextOptionsBuilder<ApplicationUsersContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            using(var context = new ApplicationUsersContext(options))
            {
                u.Discount.BronzeTier = model.bronzeTier;
                u.Discount.SilverTier = model.silverTier;
                u.Discount.GoldTier = model.goldTier;
                u.Discount.DiscountPercent = model.discountPercent;
                await _userManager.UpdateAsync(u);
                context.Update(u);
                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return BadRequest(new { message = "Amenity had already been modified, please reload the page." });
                }
            }
            return Ok();

        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("SaveNewDiscountRange")]
        public async Task<object> SaveNewDiscountRange(AddDiscountRangeModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return BadRequest(new { message = "Forbbiden action for this role." });
            }

            var companies = _context2.RentACarCompanies
                .Include(comp => comp.Cars).ThenInclude(car => car.RentedDates)
                .Include(comp => comp.QuickReservations).ThenInclude(res => res.DiscountedCar)
                .Include(comp => comp.QuickReservations).ThenInclude(res => res.Dates)
                .ToList();

            foreach(var comp in companies)
            {
                if(comp.ID == model.companyId)
                {
                    foreach (var carr in comp.Cars)
                    {
                        if(carr.ID == model.carId)
                        {
                            if(!CheckAvailability(carr, model.dateFrom, model.dateTo))
                            {
                                return BadRequest(new { message = "Car is rented for specified date range." });
                            }
                            else
                            {
                                List<Date> dates = GetDates(model.dateFrom, model.dateTo);
                                comp.QuickReservations.Add(new QuickReservation() { Dates = dates, DiscountedCar = carr });
                                _context2.Update(comp);
                                await _context2.SaveChangesAsync();

                                List<CarModelAdmin> cars = new List<CarModelAdmin>();
                                foreach (var car in comp.Cars)
                                {
                                    if (!car.Removed)
                                    {
                                        List<string> rented = new List<string>();
                                        foreach (var date in car.RentedDates)
                                        {
                                            rented.Add(date.DateStr);
                                        }
                                        List<QuickReservationModel> discount = new List<QuickReservationModel>();
                                        foreach (var res in comp.QuickReservations)
                                        {
                                            if (res.DiscountedCar.ID == car.ID)
                                            {
                                                discount.Add(new QuickReservationModel() { id = res.Id, carId = res.DiscountedCar.ID, from = res.Dates[0].DateStr, to = res.Dates[res.Dates.Count - 1].DateStr });
                                            }
                                        }
                                        cars.Add(new CarModelAdmin() { avrageRating = car.AvrageRating, brand = car.Brand, id = car.ID, image = car.Image, location = car.Location, model = car.Model, passengers = car.Passengers, price = car.PricePerDay, type = car.Type, year = car.Year, rentedDates = rented, quickReservations = discount });
                                    }
                                    
                                }

                                return cars;
                            }
                        }
                    }
                }
            }
            return Ok();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("RemoveDiscountRange")]
        public async Task<object> RemoveDiscountRange(HelpModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return BadRequest(new { message = "Forbbiden action for this role." });
            }

            var options = new DbContextOptionsBuilder<ApplicationUsersContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            using (var context = new ApplicationUsersContext(options))
            {
                var companies = context.RentACarCompanies
                .Include(comp => comp.Cars).ThenInclude(car => car.RentedDates)
                .Include(comp => comp.QuickReservations).ThenInclude(res => res.DiscountedCar)
                .Include(comp => comp.QuickReservations).ThenInclude(res => res.Dates)
                .ToList();

                foreach (var comp in companies)
                {
                    foreach (var res in comp.QuickReservations)
                    {
                        if (res.Id == model.id)
                        {
                            foreach (var carr in comp.Cars)
                            {
                                if (res.DiscountedCar.ID == carr.ID)
                                {
                                    if (CheckAvailability(carr, res.Dates[0].DateStr, res.Dates[res.Dates.Count - 1].DateStr))
                                    {
                                        comp.QuickReservations.Remove(res);
                                        context.Update(comp);
                                        try
                                        {
                                            await context.SaveChangesAsync();
                                        }
                                        catch (DbUpdateConcurrencyException ex)
                                        {
                                            return BadRequest(new { message = "Car has been modified, please reload the page." });
                                        }

                                        List<CarModelAdmin> cars = new List<CarModelAdmin>();
                                        foreach (var car in comp.Cars)
                                        {
                                            if (!car.Removed)
                                            {
                                                List<string> rented = new List<string>();
                                                foreach (var date in car.RentedDates)
                                                {
                                                    rented.Add(date.DateStr);
                                                }
                                                List<QuickReservationModel> discount = new List<QuickReservationModel>();
                                                foreach (var ress in comp.QuickReservations)
                                                {
                                                    if (ress.DiscountedCar.ID == car.ID)
                                                    {
                                                        discount.Add(new QuickReservationModel() { id = ress.Id, carId = ress.DiscountedCar.ID, from = ress.Dates[0].DateStr, to = ress.Dates[ress.Dates.Count - 1].DateStr });
                                                    }
                                                }
                                                cars.Add(new CarModelAdmin() { avrageRating = car.AvrageRating, brand = car.Brand, id = car.ID, image = car.Image, location = car.Location, model = car.Model, passengers = car.Passengers, price = car.PricePerDay, type = car.Type, year = car.Year, rentedDates = rented, quickReservations = discount });
                                            }
                                            
                                        }

                                        return cars;
                                    }
                                    else
                                    {
                                        return BadRequest(new { message = "Car is rented during specified time, not able to delete now." });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return BadRequest(new { message = "Something went wrong, please try again later." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("GetProfit")]
        public async Task<object> GetProfit(ProfitModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return BadRequest(new { message = "Forbbiden action for this role." });
            }

            var options = new DbContextOptionsBuilder<ApplicationUsersContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            using (var context = new ApplicationUsersContext(options))
            {
                var companies = context.RentACarCompanies
                .Include(comp => comp.Reservations).ToList();

                var comp = await context.RentACarCompanies.FindAsync(model.company);

                double total = 0;
                foreach (var res in comp.Reservations)
                {
                    if (res.TimeStamp > DateTime.Parse(model.date1) && res.TimeStamp < DateTime.Parse(model.date2))
                    {
                        total += res.TotalPrice;
                    }
                }
                return total;
            }
        }

        [HttpPost]
        [Route("AvailableCarsAdmin")]
        public async Task<object> AvailableCarsAdmin(AvailableCarsModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return BadRequest(new { message = "Forbbiden action for this role." });
            }

            var options = new DbContextOptionsBuilder<ApplicationUsersContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            List<int> ret = new List<int>();

            using (var context = new ApplicationUsersContext(options))
            {
                var companies = context.RentACarCompanies
                .Include(comp => comp.Cars)
                    .ThenInclude(car => car.RentedDates)
                .ToList();

                var company = await context.RentACarCompanies.FindAsync(model.company);

                foreach (var car in company.Cars)
                {
                    if (CheckAvailability(car, model.from, model.to))
                    {
                        ret.Add(car.ID);
                    }
                }
            }

            return ret;
        }

        

        private bool CheckAvailability(Car car, string from, string to)
        {
            bool available = true;
            DateTime fromDate = DateTime.Parse(from);
            DateTime toDate = DateTime.Parse(to);

            foreach (var date in car.RentedDates)
            {
                DateTime dt = DateTime.Parse(date.DateStr);
                if (dt >= fromDate && dt <= toDate)
                {
                    available = false;
                    break;
                }
            }

            return available;
        }

        private bool CheckQuickReservations(RentACar comp, Car car, string from, string to)
        {
            bool available = true;
            DateTime fromDate = DateTime.Parse(from);
            DateTime toDate = DateTime.Parse(to);

            List<DateTime> dates = GetDateTimeList(from, to);

            foreach(var res in comp.QuickReservations)
            {
                if(res.DiscountedCar.ID == car.ID)
                {
                    foreach (var d in dates)
                    {
                        List<DateTime> pom = GetDateTimeList(res.Dates[0].DateStr, res.Dates[res.Dates.Count - 1].DateStr);
                        if(pom.Contains(d))
                        {
                            return false;
                        }
                    }
                }
            }

            return available;
        }

        private List<Date> GetDates(string from, string to)
        {
            List<Date> dates = new List<Date>();

            DateTime fromDate = DateTime.Parse(from);
            DateTime toDate = DateTime.Parse(to);

            dates.Add(new Date() { DateStr = fromDate.ToString() });

            while (fromDate < toDate)
            {
                DateTime d = fromDate.AddDays(1);
                dates.Add(new Date() { DateStr = d.ToString() });
                fromDate = fromDate.AddDays(1);
            }

            return dates;
        }

        private List<DateTime> GetDateTimeList(string from, string to)
        {
            List<DateTime> dates = new List<DateTime>();

            DateTime fromDate = DateTime.Parse(from);
            DateTime toDate = DateTime.Parse(to);

            dates.Add(fromDate);

            while (fromDate < toDate)
            {
                DateTime d = fromDate.AddDays(1);
                dates.Add(d);
                fromDate = fromDate.AddDays(1);
            }

            return dates;
        }

        private DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }


    }
}