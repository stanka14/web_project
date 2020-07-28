using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IdentityModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekat_BackEnd.Models.ContextData;
using Projekat_BackEnd.Models.Model;
using Microsoft.AspNetCore.Identity;

namespace Projekat_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirlineController : Controller
    {
        private ApplicationUsersContext _context;
        private UserManager<User> _userManager;


        public AirlineController(ApplicationUsersContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("GetAllActivatedCompanies")]
        public IEnumerable<AirlineListingInfoModel> GetAllActivatedCompanies()
        {
            List<Airline> companies = _context.AirlineCompanies.ToList();
            List<AirlineListingInfoModel> retCompanies = new List<AirlineListingInfoModel>();
            foreach (var comp in companies)
            {
                retCompanies.Add(new AirlineListingInfoModel() { Id = comp.Id, Img = comp.Img, MainLocation = comp.Address, Name = comp.Name, Rating = comp.Rating });
            }
            return retCompanies;
        }
        [HttpPost]
        [Route("LoadFlight")]
        public FlightListingInfoModel LoadFlight(IdModel2 idModel)
        {
            FlightListingInfoModel ret = new FlightListingInfoModel();
            List<Airline> companies = new List<Airline>();
            try
            {
                    companies = _context.AirlineCompanies
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
            catch(Exception e)
            {

            }
            /*string userid = User.Claims.First(c => c.Type == "UserID").Value;
            string role = User.Claims.First(c => c.Type == "Roles").Value;


            var user = await _userManager.FindByIdAsync(userid);
            */
         

            for (int i = 0; i < companies.Count(); i++)
            {
                for (int j = 0; j < companies[i].Flights.Count(); j++)
                {

                    if (companies[i].Flights[j].Id == idModel.Id)
                    {

                        FlightListingInfoModel fi = new FlightListingInfoModel();
                       /* if (role == "AirlineAdministrator" && user.CompanyId == companies[i].Id)
                            fi.isAdmin = true;
                        else
                            fi.isAdmin = false;*/

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
                            ti.Add(new TicketListingInfoModel() { discount = s.Discount, id = s.Id, flight = new FlightTicket()
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

                           , seat = s.Seat });
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
        [HttpGet]
        [Route("GetAllFlights")]
        public IEnumerable<FlightListingInfoModel> GetAllFlights()
        {
            List<Airline> companies = _context.AirlineCompanies.Include(comp => comp.Flights)
                .ThenInclude(f => f.Luggage)
                .Include(fs => fs.Flights).ThenInclude(d => d.From).Include(fs => fs.Flights)
                .ThenInclude(d => d.To).ToList();
            List<FlightListingInfoModel> retCompanies = new List<FlightListingInfoModel>();
            foreach (var comp in companies)
            {
                foreach (var f in comp.Flights)
                {

                    FlightListingInfoModel fi = new FlightListingInfoModel();

                    List<DestinationListingInfoModel> pomd = new List<DestinationListingInfoModel>();
                    if (f.Stops != null)
                    {
                        foreach (var sh in f.Stops)
                        {
                            pomd.Add(new DestinationListingInfoModel() { description = sh.Description, id = sh.Id, img = sh.Img, name = sh.Name });
                        }
                    }
                    else
                    {
                        pomd = new List<DestinationListingInfoModel>();
                    }
                    retCompanies.Add(new FlightListingInfoModel() { stops = pomd, airlineId = comp.Id, to = new DestinationListingInfoModel() { id = f.To.Id, description = f.To.Description, img = f.To.Img, name = f.To.Name }, from = new DestinationListingInfoModel() { id = f.From.Id, description = f.From.Description, img = f.From.Img, name = f.From.Name }, extra = f.Extra, luggage = new LuggageListingInfoModel() { dimensions = f.Luggage.Dimensions, quantity = f.Luggage.Quantity, weight = f.Luggage.Weight }, price = f.Price, id = f.Id, duration = f.Duration, numOfPassengers = f.NumOfPassengers, departureDate = f.DepartureDate, idCompany = f.IdCompany, povratniLet = fi });
                }
            }
            return retCompanies;
        }

        [HttpPost]
        [Route("SearchCompanies")]
        public IEnumerable<FlightListingInfoModel> SearchCompanies(SearchCompaniesModelAirline searchModel)
        {
            List<Airline> companies = _context.AirlineCompanies.Include(comp => comp.Flights).ThenInclude(f => f.Luggage)
                .Include(fs => fs.Flights).ThenInclude(d => d.From).Include(fs => fs.Flights).ThenInclude(d => d.To).ToList();

            List<FlightListingInfoModel> retCompanies = new List<FlightListingInfoModel>();


            for (int i = 0; i < companies.Count(); i++)
            {
                for (int j = 0; j < companies[i].Flights.Count(); j++)
                {
                    if (searchModel.airline1Name == "" || companies[i].Flights[j].From.Name.ToLower() == searchModel.airline1Name.ToLower())
                    {
                        if (searchModel.airline2Name == "" || companies[i].Flights[j].To.Name.ToLower() == searchModel.airline2Name.ToLower())
                        {

                            if ((searchModel.from == null || searchModel.from == "") || (DateTime.Parse(companies[i].Flights[j].DepartureDate) >= DateTime.Parse(searchModel.from)))
                            {
                                if ((searchModel.to == null || searchModel.to == "") || (DateTime.Parse(companies[i].Flights[j].DepartureDate) >= DateTime.Parse(searchModel.to)))
                                {
                                    FlightListingInfoModel fi = new FlightListingInfoModel();


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
                                    retCompanies.Add(new FlightListingInfoModel()
                                    {
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
                                        stops = pomd,
                                        price = companies[i].Flights[j].Price,
                                        id = companies[i].Flights[j].Id,
                                        duration = companies[i].Flights[j].Duration,
                                        numOfPassengers = companies[i].Flights[j].NumOfPassengers,
                                        departureDate = companies[i].Flights[j].DepartureDate,
                                        idCompany = companies[i].Flights[j].IdCompany,
                                        povratniLet = fi
                                    });
                                }
                            }
                        }
                    }
                }
            }
            return retCompanies;
        }


        [HttpPost]
        [Route("SearchFlights")]
        public IEnumerable<FlightListingInfoModel> SearchFlights(SearchFlightsModel searchModel)
        {
            List<Airline> companies = _context.AirlineCompanies.Include(comp => comp.Flights).ThenInclude(f => f.Luggage)
                .Include(fs => fs.Flights).ThenInclude(d => d.From)
                .Include(fs => fs.Flights).ThenInclude(d => d.To)
                .Include(fs => fs.Flights).ThenInclude(d => d.Seats).ToList();

            List<FlightListingInfoModel> retCompanies = new List<FlightListingInfoModel>();


            for (int i = 0; i < companies.Count(); i++)
            {
                for (int j = 0; j < companies[i].Flights.Count(); j++)
                {
                    if (searchModel.From == "" || companies[i].Flights[j].From.Name.ToLower() == searchModel.From.ToLower())
                    {
                        if (searchModel.To == "" || companies[i].Flights[j].To.Name.ToLower() == searchModel.To.ToLower())
                        {
                            if (searchModel.Prise == "" || companies[i].Flights[j].Price <= Int32.Parse(searchModel.Prise))
                            {
                                if (searchModel.Trip == "" || companies[i].Flights[j].Trip.ToString() == searchModel.Trip)
                                {
                                    if ((searchModel.Date == null || searchModel.Date == "") || (DateTime.Parse(companies[i].Flights[j].DepartureDate) >= DateTime.Parse(searchModel.Date)))
                                    {
                                        if ((searchModel.To == null || searchModel.To == "") || (DateTime.Parse(companies[i].Flights[j].DepartureDate) >= DateTime.Parse(searchModel.To)))
                                        {

                                            FlightListingInfoModel fi = new FlightListingInfoModel();

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
                                            FlightListingInfoModel povra = new FlightListingInfoModel()
                                            {
                                                stops = pomd,
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
                                                povratniLet = fi
                                            };
                                            if (searchModel.Clas == "")
                                            {
                                                retCompanies.Add(povra);
                                            }
                                            else if (searchModel.Clas == Classes.Business.ToString())
                                            {
                                                for (int f = 0; f < companies[i].Flights[j].Seats.Count(); f++)
                                                {
                                                    if (companies[i].Flights[j].Seats[f].Type == Classes.Business)
                                                    {
                                                        retCompanies.Add(povra);
                                                        break;
                                                    }
                                                }
                                            }
                                            else if (searchModel.Clas == Classes.Economy.ToString())
                                            {
                                                for (int f = 0; f < companies[i].Flights[j].Seats.Count(); f++)
                                                {
                                                    if (companies[i].Flights[j].Seats[f].Type == Classes.Business)
                                                    {
                                                        retCompanies.Add(povra);
                                                        break;
                                                    }
                                                }
                                            }
                                            else if (searchModel.Clas == Classes.First.ToString())
                                            {
                                                for (int f = 0; f < companies[i].Flights[j].Seats.Count(); f++)
                                                {
                                                    if (companies[i].Flights[j].Seats[f].Type == Classes.Business)
                                                    {
                                                        retCompanies.Add(povra);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return retCompanies;
        }
        [HttpPost]
        [Route("LoadCompany")]
        public AirlineListingInfoModel LoadCompany(IdModel2 id)
        {
            List<User> users = _context.ApplicationUsers
             
                .Include(comp => comp.Flights)
                .ToList();
            List<Airline> companies = _context.AirlineCompanies
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

            AirlineListingInfoModel retCompanies = new AirlineListingInfoModel();


            for (int i = 0; i < companies.Count(); i++)
            {
                if (companies[i].Id == id.Id)
                {
                    List<DestinationListingInfoModel> di = new List<DestinationListingInfoModel>();
                    List<DestinationListingInfoModel> di2 = new List<DestinationListingInfoModel>();

                    List<RaterListingInfoModel> ri = new List<RaterListingInfoModel>();
                    List<TicketListingInfoModel> ti = new List<TicketListingInfoModel>();
                    List<TicketListingInfoModel> soldt = new List<TicketListingInfoModel>();

                    List<FlightListingInfoModel> fl = new List<FlightListingInfoModel>();
                    foreach (var f in companies[i].Flights)
                    {

                        List<SeatListingInfoModel> si = new List<SeatListingInfoModel>();

                        FlightListingInfoModel fi = new FlightListingInfoModel();

                        List<DestinationListingInfoModel> pomd = new List<DestinationListingInfoModel>();
                        foreach (var s in f.Seats)
                        {
                            si.Add(new SeatListingInfoModel() { isSelected = s.IsSelected, id = s.Id, taken = s.Taken, traveller = new TravellerInfo(), type = s.Type });
                        }
                        if (f.Stops != null)
                        {
                            foreach (var sh in f.Stops)
                            {
                                pomd.Add(new DestinationListingInfoModel() { description = sh.Description, id = sh.Id, img = sh.Img, name = sh.Name });
                            }
                        }
                        else
                        {
                            pomd = new List<DestinationListingInfoModel>();
                        }

                        if (f.SoldTickets != null)
                        {
                            foreach (var sh in f.SoldTickets)
                            {
                                soldt.Add(new TicketListingInfoModel() {discount = sh.Discount });
                            }
                        }
                        else
                        {
                            pomd = new List<DestinationListingInfoModel>();
                        }
                        fl.Add(new FlightListingInfoModel() { seats = si, stops = pomd, airlineId = companies[i].Id, to = new DestinationListingInfoModel() { id = f.To.Id, description = f.To.Description, img = f.To.Img, name = f.To.Name }, from = new DestinationListingInfoModel() { id = f.From.Id, description = f.From.Description, img = f.From.Img, name = f.From.Name },
                            extra = f.Extra,
                            luggage = new LuggageListingInfoModel() { dimensions = f.Luggage.Dimensions, quantity = f.Luggage.Quantity, weight = f.Luggage.Weight },
                            price = f.Price, id = f.Id, duration = f.Duration, numOfPassengers = f.NumOfPassengers, departureDate = f.DepartureDate,
                            idCompany = f.IdCompany, povratniLet = fi, rate = f.Rate,
                        soldTickets = soldt});
                    }

                    foreach (var s in companies[i].PopDestinaations)
                    {
                        di.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.destination.Description,
                            img = s.destination.Img
                        ,
                            name = s.destination.Name
                        });
                    }

                    foreach (var s in companies[i].Destinations)
                    {
                        di2.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.Description,
                            img = s.Img,
                            name = s.Name
                        });
                    }
                    foreach (var s in companies[i].PopDestinaations)
                    {
                        di.Add(new DestinationListingInfoModel() { id = s.Id, description = s.destination.Description, img = s.destination.Img
                        , name = s.destination.Name });
                    }
                    foreach (var s in companies[i].Raters)
                    {
                        ri.Add(new RaterListingInfoModel() { id = s.Id, rate = s.Rate });
                    }
                    foreach (var s in companies[i].FastTickets)
                    {
                        bool moze = true;
                        foreach (var usr in users)
                        {
                            if (usr.Flights == null)
                                usr.Flights = new List<Ticket>();
                            if (usr.Flights.Contains(s))
                                moze = false;
                        }
                        if (moze)
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
                    }

                    retCompanies = (new AirlineListingInfoModel() { Id = companies[i].Id,
                        Img = companies[i].Img, MainLocation = companies[i].Address,
                        Name = companies[i].Name, Rating = companies[i].Rating, Lat = companies[i]
                    .Lat, Lon = companies[i].Lon, Admin = companies[i].Admin, Description = companies[i].Description,
                        Raters = ri, Destinations = di2, PopDestinaations = di, FastTickets = ti, Flights = fl
                    });
                }
            }

            return retCompanies;
        }


        [HttpPost]
        [Route("UpdateCompany")]
        public AirlineListingInfoModel UpdateCompany(UpdateCompanyModel id)
        {

            List<Airline> companies = _context.AirlineCompanies
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

            AirlineListingInfoModel retCompanies = new AirlineListingInfoModel();

            for (int i = 0; i < companies.Count(); i++)
            {
                if (companies[i].Id == id.id)
                {
                    companies[i].Img = id.img;
                    companies[i].Name = id.name;
                    companies[i].Description = id.description;
                    _context.SaveChanges();
                }
            }
            for (int i = 0; i < companies.Count(); i++)
            {
                if (companies[i].Id == id.id)
                {
                    List<DestinationListingInfoModel> di = new List<DestinationListingInfoModel>();
                    List<DestinationListingInfoModel> di2 = new List<DestinationListingInfoModel>();

                    List<RaterListingInfoModel> ri = new List<RaterListingInfoModel>();
                    List<TicketListingInfoModel> ti = new List<TicketListingInfoModel>();


                    List<FlightListingInfoModel> fl = new List<FlightListingInfoModel>();
                    List<SeatListingInfoModel> si = new List<SeatListingInfoModel>();

                    foreach (var f in companies[i].Flights)
                    {
                        FlightListingInfoModel fi = new FlightListingInfoModel();

                        List<DestinationListingInfoModel> pomd = new List<DestinationListingInfoModel>();
                        if (f.Stops != null)
                        {
                            foreach (var sh in f.Stops)
                            {
                                pomd.Add(new DestinationListingInfoModel() { description = sh.Description, id = sh.Id, img = sh.Img, name = sh.Name });
                            }
                        }
                        else
                        {
                            pomd = new List<DestinationListingInfoModel>();
                        }
                        fl.Add(new FlightListingInfoModel()
                        {
                            seats = si,
                            stops = pomd,
                            airlineId = companies[i].Id,
                            to = new DestinationListingInfoModel() { id = f.To.Id, description = f.To.Description, img = f.To.Img, name = f.To.Name },
                            from = new DestinationListingInfoModel() { id = f.From.Id, description = f.From.Description, img = f.From.Img, name = f.From.Name },
                            extra = f.Extra,
                            luggage = new LuggageListingInfoModel() { dimensions = f.Luggage.Dimensions, quantity = f.Luggage.Quantity, weight = f.Luggage.Weight },
                            price = f.Price,
                            id = f.Id,
                            duration = f.Duration,
                            numOfPassengers = f.NumOfPassengers,
                            departureDate = f.DepartureDate,
                            idCompany = f.IdCompany,
                            povratniLet = fi
                        });
                    }

                    foreach (var s in companies[i].PopDestinaations)
                    {
                        di.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.destination.Description,
                            img = s.destination.Img
                        ,
                            name = s.destination.Name
                        });
                    }

                    foreach (var s in companies[i].Destinations)
                    {
                        di2.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.Description,
                            img = s.Img,
                            name = s.Name
                        });
                    }
                    foreach (var s in companies[i].PopDestinaations)
                    {
                        di.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.destination.Description,
                            img = s.destination.Img
                        ,
                            name = s.destination.Name
                        });
                    }
                    foreach (var s in companies[i].Raters)
                    {
                        ri.Add(new RaterListingInfoModel() { id = s.Id, rate = s.Rate });
                    }
                    foreach (var s in companies[i].FastTickets)
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

                    retCompanies = (new AirlineListingInfoModel()
                    {
                        Id = companies[i].Id,
                        Img = companies[i].Img,
                        MainLocation = companies[i].Address,
                        Name = companies[i].Name,
                        Rating = companies[i].Rating,
                        Lat = companies[i]
                    .Lat,
                        Lon = companies[i].Lon,
                        Admin = companies[i].Admin,
                        Description = companies[i].Description,
                        Raters = ri,
                        Destinations = di2,
                        PopDestinaations = di,
                        FastTickets = ti,
                        Flights = fl
                    });
                }
            }

            return retCompanies;
        }


        [HttpPost]
        [Route("RemoveFlight")]
        public AirlineListingInfoModel RemoveFlight(IdModel model)
        {

            List<Airline> companies = _context.AirlineCompanies
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
                .Include(fs => fs.Flights).ThenInclude(d => d.Povratnilet)
                .ToList();
            List<User> users = _context.ApplicationUsers.
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
            bool neBrisi = false;
            foreach (var r in users)
            {
                foreach (var f in r.Flights)
                {
                    if (f.Flight.Id == model.id)
                    {
                        neBrisi = true;
                    }
                }
            }
            AirlineListingInfoModel retCompanies = new AirlineListingInfoModel();
            int pomId = 0;
            for (int i = 0; i < companies.Count(); i++)
            {
                for (int d = 0; d < companies[i].Flights.Count(); d++)
                {
                    if (companies[i].Flights[d].Id == model.id)
                    {
                        pomId = companies[i].Id;
                        if (!neBrisi)
                        {
                            companies[i].Flights.Remove(companies[i].Flights[d]);
                            _context.SaveChanges();
                        }
                    }
                }
            }


            for (int i = 0; i < companies.Count(); i++)
            {

                if (companies[i].Id == pomId)
                {
                    List<DestinationListingInfoModel> di = new List<DestinationListingInfoModel>();
                    List<DestinationListingInfoModel> di2 = new List<DestinationListingInfoModel>();

                    List<RaterListingInfoModel> ri = new List<RaterListingInfoModel>();
                    List<TicketListingInfoModel> ti = new List<TicketListingInfoModel>();


                    List<FlightListingInfoModel> fl = new List<FlightListingInfoModel>();
                    List<SeatListingInfoModel> si = new List<SeatListingInfoModel>();

                    foreach (var f in companies[i].Flights)
                    {
                        FlightListingInfoModel fi = new FlightListingInfoModel();

                        List<DestinationListingInfoModel> pomd = new List<DestinationListingInfoModel>();
                        if (f.Stops != null)
                        {
                            foreach (var sh in f.Stops)
                            {
                                pomd.Add(new DestinationListingInfoModel() { description = sh.Description, id = sh.Id, img = sh.Img, name = sh.Name });
                            }
                        }
                        else
                        {
                            pomd = new List<DestinationListingInfoModel>();
                        }
                        fl.Add(new FlightListingInfoModel()
                        {
                            seats = si,
                            stops = pomd,
                            airlineId = companies[i].Id,
                            to = new DestinationListingInfoModel() { id = f.To.Id, description = f.To.Description, img = f.To.Img, name = f.To.Name },
                            from = new DestinationListingInfoModel() { id = f.From.Id, description = f.From.Description, img = f.From.Img, name = f.From.Name },
                            extra = f.Extra,
                            luggage = new LuggageListingInfoModel() { dimensions = f.Luggage.Dimensions, quantity = f.Luggage.Quantity, weight = f.Luggage.Weight },
                            price = f.Price,
                            id = f.Id,
                            duration = f.Duration,
                            numOfPassengers = f.NumOfPassengers,
                            departureDate = f.DepartureDate,
                            idCompany = f.IdCompany,
                            povratniLet = fi
                        });
                    }

                    foreach (var s in companies[i].PopDestinaations)
                    {
                        di.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.destination.Description,
                            img = s.destination.Img
                        ,
                            name = s.destination.Name
                        });
                    }

                    foreach (var s in companies[i].Destinations)
                    {
                        di2.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.Description,
                            img = s.Img,
                            name = s.Name
                        });
                    }
                    foreach (var s in companies[i].PopDestinaations)
                    {
                        di.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.destination.Description,
                            img = s.destination.Img
                        ,
                            name = s.destination.Name
                        });
                    }
                    foreach (var s in companies[i].Raters)
                    {
                        ri.Add(new RaterListingInfoModel() { id = s.Id, rate = s.Rate });
                    }
                    foreach (var s in companies[i].FastTickets)
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

                    retCompanies = (new AirlineListingInfoModel()
                    {
                        Id = companies[i].Id,
                        Img = companies[i].Img,
                        MainLocation = companies[i].Address,
                        Name = companies[i].Name,
                        Rating = companies[i].Rating,
                        Lat = companies[i]
                    .Lat,
                        Lon = companies[i].Lon,
                        Admin = companies[i].Admin,
                        Description = companies[i].Description,
                        Raters = ri,
                        Destinations = di2,
                        PopDestinaations = di,
                        FastTickets = ti,
                        Flights = fl
                    });
                }
            }

            return retCompanies;
        }
        [HttpPost]
        [Route("RemoveLocation")]
        public AirlineListingInfoModel RemoveLocation(Destination model)
        {

            List<Airline> companies = _context.AirlineCompanies
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

            AirlineListingInfoModel retCompanies = new AirlineListingInfoModel();
            int pomId = 0;
            for (int i = 0; i < companies.Count(); i++)
            {
                for (int d = 0; d < companies[i].Destinations.Count(); d++)
                {
                    if (companies[i].Destinations[d].Id == model.Id)
                    {
                        pomId = companies[i].Id;
                        companies[i].Destinations.Remove(companies[i].Destinations[d]);
                        _context.SaveChanges();
                    }
                }
            }


            for (int i = 0; i < companies.Count(); i++)
            {

                if (companies[i].Id == pomId)
                {
                    List<DestinationListingInfoModel> di = new List<DestinationListingInfoModel>();
                    List<DestinationListingInfoModel> di2 = new List<DestinationListingInfoModel>();

                    List<RaterListingInfoModel> ri = new List<RaterListingInfoModel>();
                    List<TicketListingInfoModel> ti = new List<TicketListingInfoModel>();


                    List<FlightListingInfoModel> fl = new List<FlightListingInfoModel>();
                    List<SeatListingInfoModel> si = new List<SeatListingInfoModel>();

                    foreach (var f in companies[i].Flights)
                    {
                        FlightListingInfoModel fi = new FlightListingInfoModel();

                        List<DestinationListingInfoModel> pomd = new List<DestinationListingInfoModel>();
                        if (f.Stops != null)
                        {
                            foreach (var sh in f.Stops)
                            {
                                pomd.Add(new DestinationListingInfoModel() { description = sh.Description, id = sh.Id, img = sh.Img, name = sh.Name });
                            }
                        }
                        else
                        {
                            pomd = new List<DestinationListingInfoModel>();
                        }
                        fl.Add(new FlightListingInfoModel()
                        {
                            seats = si,
                            stops = pomd,
                            airlineId = companies[i].Id,
                            to = new DestinationListingInfoModel() { id = f.To.Id, description = f.To.Description, img = f.To.Img, name = f.To.Name },
                            from = new DestinationListingInfoModel() { id = f.From.Id, description = f.From.Description, img = f.From.Img, name = f.From.Name },
                            extra = f.Extra,
                            luggage = new LuggageListingInfoModel() { dimensions = f.Luggage.Dimensions, quantity = f.Luggage.Quantity, weight = f.Luggage.Weight },
                            price = f.Price,
                            id = f.Id,
                            duration = f.Duration,
                            numOfPassengers = f.NumOfPassengers,
                            departureDate = f.DepartureDate,
                            idCompany = f.IdCompany,
                            povratniLet = fi
                        });
                    }

                    foreach (var s in companies[i].PopDestinaations)
                    {
                        di.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.destination.Description,
                            img = s.destination.Img
                        ,
                            name = s.destination.Name
                        });
                    }

                    foreach (var s in companies[i].Destinations)
                    {
                        di2.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.Description,
                            img = s.Img,
                            name = s.Name
                        });
                    }
                    foreach (var s in companies[i].PopDestinaations)
                    {
                        di.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.destination.Description,
                            img = s.destination.Img
                        ,
                            name = s.destination.Name
                        });
                    }
                    foreach (var s in companies[i].Raters)
                    {
                        ri.Add(new RaterListingInfoModel() { id = s.Id, rate = s.Rate });
                    }
                    foreach (var s in companies[i].FastTickets)
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

                    retCompanies = (new AirlineListingInfoModel()
                    {
                        Id = companies[i].Id,
                        Img = companies[i].Img,
                        MainLocation = companies[i].Address,
                        Name = companies[i].Name,
                        Rating = companies[i].Rating,
                        Lat = companies[i]
                    .Lat,
                        Lon = companies[i].Lon,
                        Admin = companies[i].Admin,
                        Description = companies[i].Description,
                        Raters = ri,
                        Destinations = di2,
                        PopDestinaations = di,
                        FastTickets = ti,
                        Flights = fl
                    });
                }
            }

            return retCompanies;
        }

      
        [HttpPost]
        [Route("NewFastTicket")]
        public AirlineListingInfoModel NewFastTicket(NewFastTicket model)
        {
            List<Airline> companies = _context.AirlineCompanies
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

            AirlineListingInfoModel retCompanies = new AirlineListingInfoModel();

            int pomId = 0;
            for (int i = 0; i < companies.Count(); i++)
            {
                for (int d = 0; d < companies[i].Flights.Count(); d++)
                {
                    if (companies[i].Flights[d].Id == model.flightId)
                    {
                        pomId = companies[i].Id;
                        foreach (var s in companies[i].Flights[d].Seats)
                        {
                            if (s.Id == model.seatId && !s.Taken)
                            {
                                s.Taken = true;
                                Ticket t = new Ticket() { Seat = s, Discount = model.discount, Flight = companies[i].Flights[d] };
                                  companies[i].FastTickets.Add(t);
                                _context.SaveChanges();

                            }
                        }
                    }                    
                }
            }


            for (int i = 0; i < companies.Count(); i++)
            {

                if (companies[i].Id == pomId)
                {
                    List<DestinationListingInfoModel> di = new List<DestinationListingInfoModel>();
                    List<DestinationListingInfoModel> di2 = new List<DestinationListingInfoModel>();

                    List<RaterListingInfoModel> ri = new List<RaterListingInfoModel>();
                    List<TicketListingInfoModel> ti = new List<TicketListingInfoModel>();


                    List<FlightListingInfoModel> fl = new List<FlightListingInfoModel>();
                    List<SeatListingInfoModel> si = new List<SeatListingInfoModel>();

                    foreach (var f in companies[i].Flights)
                    {
                        FlightListingInfoModel fi = new FlightListingInfoModel();

                        List<DestinationListingInfoModel> pomd = new List<DestinationListingInfoModel>();
                        if (f.Stops != null)
                        {
                            foreach (var sh in f.Stops)
                            {
                                pomd.Add(new DestinationListingInfoModel() { description = sh.Description, id = sh.Id, img = sh.Img, name = sh.Name });
                            }
                        }
                        else
                        {
                            pomd = new List<DestinationListingInfoModel>();
                        }
                        fl.Add(new FlightListingInfoModel()
                        {
                            seats = si,
                            stops = pomd,
                            airlineId = companies[i].Id,
                            to = new DestinationListingInfoModel() { id = f.To.Id, description = f.To.Description, img = f.To.Img, name = f.To.Name },
                            from = new DestinationListingInfoModel() { id = f.From.Id, description = f.From.Description, img = f.From.Img, name = f.From.Name },
                            extra = f.Extra,
                            luggage = new LuggageListingInfoModel() { dimensions = f.Luggage.Dimensions, quantity = f.Luggage.Quantity, weight = f.Luggage.Weight },
                            price = f.Price,
                            id = f.Id,
                            duration = f.Duration,
                            numOfPassengers = f.NumOfPassengers,
                            departureDate = f.DepartureDate,
                            idCompany = f.IdCompany,
                            povratniLet = fi
                        });
                    }

                    foreach (var s in companies[i].PopDestinaations)
                    {
                        di.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.destination.Description,
                            img = s.destination.Img
                        ,
                            name = s.destination.Name
                        });
                    }

                    foreach (var s in companies[i].Destinations)
                    {
                        di2.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.Description,
                            img = s.Img,
                            name = s.Name
                        });
                    }
                    foreach (var s in companies[i].PopDestinaations)
                    {
                        di.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.destination.Description,
                            img = s.destination.Img
                        ,
                            name = s.destination.Name
                        });
                    }
                    foreach (var s in companies[i].Raters)
                    {
                        ri.Add(new RaterListingInfoModel() { id = s.Id, rate = s.Rate });
                    }
                    foreach (var s in companies[i].FastTickets)
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

                    retCompanies = (new AirlineListingInfoModel()
                    {
                        Id = companies[i].Id,
                        Img = companies[i].Img,
                        MainLocation = companies[i].Address,
                        Name = companies[i].Name,
                        Rating = companies[i].Rating,
                        Lat = companies[i]
                    .Lat,
                        Lon = companies[i].Lon,
                        Admin = companies[i].Admin,
                        Description = companies[i].Description,
                        Raters = ri,
                        Destinations = di2,
                        PopDestinaations = di,
                        FastTickets = ti,
                        Flights = fl
                    });
                }
            }

            return retCompanies;
        }
        
        [HttpPost]
        [Route("AddToPopular")]
        public AirlineListingInfoModel AddToPopular(Destination model)
        {

            List<Airline> companies = _context.AirlineCompanies
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

            AirlineListingInfoModel retCompanies = new AirlineListingInfoModel();
            int pomId = 0;
            for (int i = 0; i < companies.Count(); i++)
            {
                for (int d = 0; d < companies[i].Destinations.Count(); d++)
                {
                    if (companies[i].Destinations[d].Id == model.Id)
                    {
                        pomId = companies[i].Id;
                        if (companies[i].PopDestinaations == null)
                            companies[i].PopDestinaations = new List<DestinationPopular>();
                        DestinationPopular dp = new DestinationPopular() { Id = companies[i].Destinations[d].Id, destination = companies[i].Destinations[d] };
                        if (!companies[i].PopDestinaations.Contains(dp))
                        {
                            companies[i].PopDestinaations.Add(dp);
                            _context.SaveChanges();
                        }
                    }
                }
            }


            for (int i = 0; i < companies.Count(); i++)
            {

                if (companies[i].Id == pomId)
                {
                    List<DestinationListingInfoModel> di = new List<DestinationListingInfoModel>();
                    List<DestinationListingInfoModel> di2 = new List<DestinationListingInfoModel>();

                    List<RaterListingInfoModel> ri = new List<RaterListingInfoModel>();
                    List<TicketListingInfoModel> ti = new List<TicketListingInfoModel>();


                    List<FlightListingInfoModel> fl = new List<FlightListingInfoModel>();
                    List<SeatListingInfoModel> si = new List<SeatListingInfoModel>();

                    foreach (var f in companies[i].Flights)
                    {
                        FlightListingInfoModel fi = new FlightListingInfoModel();
                       
                        List<DestinationListingInfoModel> pomd = new List<DestinationListingInfoModel>();
                        if (f.Stops != null)
                        {
                            foreach (var sh in f.Stops)
                            {
                                pomd.Add(new DestinationListingInfoModel() { description = sh.Description, id = sh.Id, img = sh.Img, name = sh.Name });
                            }
                        }
                        else
                        {
                            pomd = new List<DestinationListingInfoModel>();
                        }
                        fl.Add(new FlightListingInfoModel()
                        {
                            seats = si,
                            stops = pomd,
                            airlineId = companies[i].Id,
                            to = new DestinationListingInfoModel() { id = f.To.Id, description = f.To.Description, img = f.To.Img, name = f.To.Name },
                            from = new DestinationListingInfoModel() { id = f.From.Id, description = f.From.Description, img = f.From.Img, name = f.From.Name },
                            extra = f.Extra,
                            luggage = new LuggageListingInfoModel() { dimensions = f.Luggage.Dimensions, quantity = f.Luggage.Quantity, weight = f.Luggage.Weight },
                            price = f.Price,
                            id = f.Id,
                            duration = f.Duration,
                            numOfPassengers = f.NumOfPassengers,
                            departureDate = f.DepartureDate,
                            idCompany = f.IdCompany,
                            povratniLet = fi
                        });
                    }

                    foreach (var s in companies[i].PopDestinaations)
                    {
                        di.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.destination.Description,
                            img = s.destination.Img
                        ,
                            name = s.destination.Name
                        });
                    }

                    foreach (var s in companies[i].Destinations)
                    {
                        di2.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.Description,
                            img = s.Img,
                            name = s.Name
                        });
                    }
                    foreach (var s in companies[i].PopDestinaations)
                    {
                        di.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.destination.Description,
                            img = s.destination.Img
                        ,
                            name = s.destination.Name
                        });
                    }
                    foreach (var s in companies[i].Raters)
                    {
                        ri.Add(new RaterListingInfoModel() { id = s.Id, rate = s.Rate });
                    }
                    foreach (var s in companies[i].FastTickets)
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

                    retCompanies = (new AirlineListingInfoModel()
                    {
                        Id = companies[i].Id,
                        Img = companies[i].Img,
                        MainLocation = companies[i].Address,
                        Name = companies[i].Name,
                        Rating = companies[i].Rating,
                        Lat = companies[i]
                    .Lat,
                        Lon = companies[i].Lon,
                        Admin = companies[i].Admin,
                        Description = companies[i].Description,
                        Raters = ri,
                        Destinations = di2,
                        PopDestinaations = di,
                        FastTickets = ti,
                        Flights = fl
                    });
                }
            }

            return retCompanies;
        }
        [HttpPost]
        [Route("EditFlight")]
        public AirlineListingInfoModel EditFlight(EditFlightModel model)
        {
            AirlineListingInfoModel retCompanies = new AirlineListingInfoModel();

            List<Airline> companies = _context.AirlineCompanies
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


            int pomId = 0;
            for (int i = 0; i < companies.Count(); i++)
            {
                for(int j = 0;j < companies[i].Flights.Count();j++)
                {
                    if(companies[i].Flights[j].Id == model.id)
                    {
                        pomId = companies[i].Id;
                        companies[i].Flights[j].Price = model.price;
                        companies[i].Flights[j].Extra = model.extra;
                        companies[i].Flights[j].Luggage.Weight = model.lug;
                        var date = new DateTime();
                        if (model.date != "")
                        {
                            date = DateTime.Parse(model.date);
                            TimeSpan ts = new TimeSpan(Int32.Parse(model.date2.Split(':')[0]), Int32.Parse(model.date2.Split(':')[1]), 0);
                            date = date.Date + ts;
                        }
                        else
                        {
                            date = DateTime.Parse(companies[i].Flights[j].DepartureDate);
                            TimeSpan ts = new TimeSpan(Int32.Parse(model.date2.Split(':')[0]), Int32.Parse(model.date2.Split(':')[1]), 0);
                            date = date.Date + ts;
                        }
                        companies[i].Flights[j].DepartureDate = date.ToString();

                        _context.SaveChanges();
                    }
                }
            }
         
             
               

            for (int i = 0; i < companies.Count(); i++)
            {

                if (companies[i].Id == pomId)
                {
                    List<DestinationListingInfoModel> di = new List<DestinationListingInfoModel>();
                    List<DestinationListingInfoModel> di2 = new List<DestinationListingInfoModel>();

                    List<RaterListingInfoModel> ri = new List<RaterListingInfoModel>();
                    List<TicketListingInfoModel> ti = new List<TicketListingInfoModel>();


                    List<FlightListingInfoModel> fl = new List<FlightListingInfoModel>();

                    foreach (var f in companies[i].Flights)
                    {
                        List<SeatListingInfoModel> si = new List<SeatListingInfoModel>();

                        FlightListingInfoModel fi = new FlightListingInfoModel();

                        List<DestinationListingInfoModel> pomd = new List<DestinationListingInfoModel>();
                        if (f.Stops != null)
                        {
                            foreach (var sh in f.Stops)
                            {
                                pomd.Add(new DestinationListingInfoModel() { description = sh.Description, id = sh.Id, img = sh.Img, name = sh.Name });
                            }
                        }
                        else
                        {
                            pomd = new List<DestinationListingInfoModel>();
                        }
                        if (f.Seats != null)
                        {
                            foreach (var s in f.Seats)
                            {
                                si.Add(new SeatListingInfoModel() { isSelected = s.IsSelected, id = s.Id, taken = s.Taken, traveller = new TravellerInfo(), type = s.Type });
                            }
                        }
                        else
                        {
                            pomd = new List<DestinationListingInfoModel>();
                        }

                        fl.Add(new FlightListingInfoModel()
                        {
                            seats = si,
                            stops = pomd,
                            airlineId = companies[i].Id,
                            to = new DestinationListingInfoModel() { id = f.To.Id, description = f.To.Description, img = f.To.Img, name = f.To.Name },
                            from = new DestinationListingInfoModel() { id = f.From.Id, description = f.From.Description, img = f.From.Img, name = f.From.Name },
                            extra = f.Extra,
                            luggage = new LuggageListingInfoModel() { dimensions = f.Luggage.Dimensions, quantity = f.Luggage.Quantity, weight = f.Luggage.Weight },
                            price = f.Price,
                            id = f.Id,
                            duration = f.Duration,
                            numOfPassengers = f.NumOfPassengers,
                            departureDate = f.DepartureDate,
                            idCompany = f.IdCompany,
                            povratniLet = fi
                        });
                    }

                    foreach (var s in companies[i].PopDestinaations)
                    {
                        di.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.destination.Description,
                            img = s.destination.Img
                        ,
                            name = s.destination.Name
                        });
                    }

                    foreach (var s in companies[i].Destinations)
                    {
                        di2.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.Description,
                            img = s.Img,
                            name = s.Name
                        });
                    }
                    foreach (var s in companies[i].PopDestinaations)
                    {
                        di.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.destination.Description,
                            img = s.destination.Img
                        ,
                            name = s.destination.Name
                        });
                    }
                    foreach (var s in companies[i].Raters)
                    {
                        ri.Add(new RaterListingInfoModel() { id = s.Id, rate = s.Rate });
                    }
                    foreach (var s in companies[i].FastTickets)
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

                    retCompanies = (new AirlineListingInfoModel()
                    {
                        Id = companies[i].Id,
                        Img = companies[i].Img,
                        MainLocation = companies[i].Address,
                        Name = companies[i].Name,
                        Rating = companies[i].Rating,
                        Lat = companies[i]
                    .Lat,
                        Lon = companies[i].Lon,
                        Admin = companies[i].Admin,
                        Description = companies[i].Description,
                        Raters = ri,
                        Destinations = di2,
                        PopDestinaations = di,
                        FastTickets = ti,
                        Flights = fl
                    });
                }
            }

            return retCompanies;
        }
        [HttpPost]
        [Route("AddNewFlight")]
        public AirlineListingInfoModel AddNewFlight(AddFlightModel model)
        {
            AirlineListingInfoModel retCompanies = new AirlineListingInfoModel();

            List<Airline> companies = _context.AirlineCompanies
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

            var date = DateTime.Parse(model.departureDate);
            TimeSpan ts = new TimeSpan(Int32.Parse(model.departureTime.Split(':')[0]), Int32.Parse(model.departureTime.Split(':')[1]), 0);
            date = date.Date + ts;





            Destination fr = new Destination();
            Destination to = new Destination();

            for (int i = 0; i < companies.Count(); i++)
            {
                if (companies[i].Id == model.airlineId)
                {
                    foreach(var d in companies[i].Destinations)
                    {
                        if(d.Name == model.from)
                        {
                            fr = d;
                        }
                        if(d.Name == model.to)
                        {
                            to = d;
                        }
                    }
                }
            }
            List<Destination> stopsList = new List<Destination>();
            foreach(var s in model.stops)
            {
                for (int i = 0; i < companies.Count(); i++)
                {
                    if (companies[i].Id == model.airlineId)
                    {
                        foreach (var d in companies[i].Destinations)
                        {
                            if (d.Name == s)
                            {
                                stopsList.Add(d);
                            }
                        }
                    }
                }
            }
            List<Destination> stopsList2 = new List<Destination>();
            foreach (var s in model.povratniLet.stops)
            {
                for (int i = 0; i < companies.Count(); i++)
                {
                    if (companies[i].Id == model.airlineId)
                    {
                        foreach (var d in companies[i].Destinations)
                        {
                            if (d.Name == s)
                            {
                                stopsList2.Add(d);
                            }
                        }
                    }
                }
            }
            List<Seat> seatsList = new List<Seat>();
            foreach (var s in model.seats)
            {

                Seat seat = new Seat() { IsSelected = s.isSelected, Taken = s.isSelected, Type = s.type };
                seatsList.Add(seat);
            }
            List<Seat> seatsList2 = new List<Seat>();
            foreach (var s in model.povratniLet.seats)
            {

                Seat seat = new Seat() { IsSelected = s.isSelected, Taken = s.isSelected, Type = s.type };
                seatsList2.Add(seat);
            }
            Trip trip = Trip.One_way;
            if(model.trip == "One way")
            {
                trip = Trip.One_way;
            }
            else
            {
                trip = Trip.Round_trip;
            }
            for (int i = 0; i < companies.Count(); i++)
            {
                if (companies[i].Id == model.airlineId)
                {
                    if (companies[i].Flights == null)
                        companies[i].Flights = new List<Flight>();
                    Flight flight = new Flight()
                    {
                        DepartureDate = date.ToString(),
                        Duration = model.duration,
                        Extra = model.extra,
                        Luggage = new Luggage() { Dimensions = model.luggage.dimensions, Quantity = model.luggage.quantity, Weight = model.luggage.weight },
                        IdCompany = model.idCompany,
                        From = fr,
                        To = to,
                        NumOfPassengers = model.seats.Count(),
                        Rate = 0,
                        SoldTickets = new List<Ticket>(),
                        Raters = new List<Rater>(),
                        Price = model.price,
                        Stops = stopsList,
                        Seats = seatsList,
                        Trip = trip,
                    };
                    
                    if (model.trip != "One way")
                    {   
                        var date2 = DateTime.Parse(model.povratniLet.departureDate);
                        TimeSpan ts2 = new TimeSpan(Int32.Parse(model.povratniLet.departureTime.Split(':')[0]), Int32.Parse(model.povratniLet.departureTime.Split(':')[1]), 0);
                        date2 = date2.Date + ts2;
                        flight.Povratnilet = new Flight()
                        {
                            DepartureDate = date2.ToString(),
                            Duration = model.povratniLet.duration,
                            Extra = model.povratniLet.extra,
                            Luggage = new Luggage() { Dimensions = model.povratniLet.luggage.dimensions, Quantity = model.povratniLet.luggage.quantity, Weight = model.povratniLet.luggage.weight },
                            IdCompany = model.idCompany,
                            From = to,
                            To = fr,
                            NumOfPassengers = model.povratniLet.seats.Count(),
                            Rate = 0,
                            SoldTickets = new List<Ticket>(),
                            Raters = new List<Rater>(),
                            Price = model.povratniLet.price,
                            Stops = stopsList2,
                            Seats = seatsList2,
                            Trip = Trip.One_way,
                        };
                        companies[i].Flights.Add(flight.Povratnilet);
                    }
                    companies[i].Flights.Add(flight);
                    _context.SaveChanges();
                }
            }

            for (int i = 0; i < companies.Count(); i++)
            {

                if (companies[i].Id == model.airlineId)
                {
                    List<DestinationListingInfoModel> di = new List<DestinationListingInfoModel>();
                    List<DestinationListingInfoModel> di2 = new List<DestinationListingInfoModel>();

                    List<RaterListingInfoModel> ri = new List<RaterListingInfoModel>();
                    List<TicketListingInfoModel> ti = new List<TicketListingInfoModel>();


                    List<FlightListingInfoModel> fl = new List<FlightListingInfoModel>();

                    foreach (var f in companies[i].Flights)
                    {
                        List<SeatListingInfoModel> si = new List<SeatListingInfoModel>();

                        FlightListingInfoModel fi = new FlightListingInfoModel();
                        
                        List<DestinationListingInfoModel> pomd = new List<DestinationListingInfoModel>();
                        if (f.Stops != null)
                        {
                            foreach (var sh in f.Stops)
                            {
                                pomd.Add(new DestinationListingInfoModel() { description = sh.Description, id = sh.Id, img = sh.Img, name = sh.Name });
                            }
                        }
                        else
                        {
                            pomd = new List<DestinationListingInfoModel>();
                        }
                        if (f.Seats != null)
                        {
                            foreach (var s in f.Seats)
                            {
                                si.Add(new SeatListingInfoModel() { isSelected = s.IsSelected, id = s.Id, taken = s.Taken, traveller = new TravellerInfo(), type = s.Type });
                            }
                        }
                        else
                        {
                            pomd = new List<DestinationListingInfoModel>();
                        }
   
                        fl.Add(new FlightListingInfoModel()
                        {
                            seats = si,
                            stops = pomd,
                            airlineId = companies[i].Id,
                            to = new DestinationListingInfoModel() { id = f.To.Id, description = f.To.Description, img = f.To.Img, name = f.To.Name },
                            from = new DestinationListingInfoModel() { id = f.From.Id, description = f.From.Description, img = f.From.Img, name = f.From.Name },
                            extra = f.Extra,
                            luggage = new LuggageListingInfoModel() { dimensions = f.Luggage.Dimensions, quantity = f.Luggage.Quantity, weight = f.Luggage.Weight },
                            price = f.Price,
                            id = f.Id,
                            duration = f.Duration,
                            numOfPassengers = f.NumOfPassengers,
                            departureDate = f.DepartureDate,
                            idCompany = f.IdCompany,
                            povratniLet = fi
                        });
                    }

                    foreach (var s in companies[i].PopDestinaations)
                    {
                        di.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.destination.Description,
                            img = s.destination.Img
                        ,
                            name = s.destination.Name
                        });
                    }

                    foreach (var s in companies[i].Destinations)
                    {
                        di2.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.Description,
                            img = s.Img,
                            name = s.Name
                        });
                    }
                    foreach (var s in companies[i].PopDestinaations)
                    {
                        di.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.destination.Description,
                            img = s.destination.Img
                        ,
                            name = s.destination.Name
                        });
                    }
                    foreach (var s in companies[i].Raters)
                    {
                        ri.Add(new RaterListingInfoModel() { id = s.Id, rate = s.Rate });
                    }
                    foreach (var s in companies[i].FastTickets)
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

                    retCompanies = (new AirlineListingInfoModel()
                    {
                        Id = companies[i].Id,
                        Img = companies[i].Img,
                        MainLocation = companies[i].Address,
                        Name = companies[i].Name,
                        Rating = companies[i].Rating,
                        Lat = companies[i]
                    .Lat,
                        Lon = companies[i].Lon,
                        Admin = companies[i].Admin,
                        Description = companies[i].Description,
                        Raters = ri,
                        Destinations = di2,
                        PopDestinaations = di,
                        FastTickets = ti,
                        Flights = fl
                    });
                }
            }

            return retCompanies;
        }
        [HttpPost]
        [Route("AddNewDestination")]
        public AirlineListingInfoModel AddNewDestination(AddDestinationModel model)
        {

            List<Airline> companies = _context.AirlineCompanies
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

            AirlineListingInfoModel retCompanies = new AirlineListingInfoModel();
            for (int i = 0; i < companies.Count(); i++)
            {
               if(companies[i].Id == model.CompanyId)
                {
                    if (companies[i].Destinations == null)
                        companies[i].Destinations = new List<Destination>();
                    companies[i].Destinations.Add(new Destination() { Description = model.Description, Name = model.Name, Img = model.Img });
                    _context.SaveChanges();
                }
            }


            for (int i = 0; i < companies.Count(); i++)
            {

                if (companies[i].Id == model.CompanyId)
                {
                    List<DestinationListingInfoModel> di = new List<DestinationListingInfoModel>();
                    List<DestinationListingInfoModel> di2 = new List<DestinationListingInfoModel>();

                    List<RaterListingInfoModel> ri = new List<RaterListingInfoModel>();
                    List<TicketListingInfoModel> ti = new List<TicketListingInfoModel>();


                    List<FlightListingInfoModel> fl = new List<FlightListingInfoModel>();
                    List<SeatListingInfoModel> si = new List<SeatListingInfoModel>();

                    foreach (var f in companies[i].Flights)
                    {
                        FlightListingInfoModel fi = new FlightListingInfoModel();
                        
                        List<DestinationListingInfoModel> pomd = new List<DestinationListingInfoModel>();
                        if (f.Stops != null)
                        {
                            foreach (var sh in f.Stops)
                            {
                                pomd.Add(new DestinationListingInfoModel() { description = sh.Description, id = sh.Id, img = sh.Img, name = sh.Name });
                            }
                        }
                        else
                        {
                            pomd = new List<DestinationListingInfoModel>();
                        }
                        fl.Add(new FlightListingInfoModel()
                        {
                            seats = si,
                            stops = pomd,
                            airlineId = companies[i].Id,
                            to = new DestinationListingInfoModel() { id = f.To.Id, description = f.To.Description, img = f.To.Img, name = f.To.Name },
                            from = new DestinationListingInfoModel() { id = f.From.Id, description = f.From.Description, img = f.From.Img, name = f.From.Name },
                            extra = f.Extra,
                            luggage = new LuggageListingInfoModel() { dimensions = f.Luggage.Dimensions, quantity = f.Luggage.Quantity, weight = f.Luggage.Weight },
                            price = f.Price,
                            id = f.Id,
                            duration = f.Duration,
                            numOfPassengers = f.NumOfPassengers,
                            departureDate = f.DepartureDate,
                            idCompany = f.IdCompany,
                            povratniLet = fi
                        });
                    }

                    foreach (var s in companies[i].PopDestinaations)
                    {
                        di.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.destination.Description,
                            img = s.destination.Img
                        ,
                            name = s.destination.Name
                        });
                    }

                    foreach (var s in companies[i].Destinations)
                    {
                        di2.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.Description,
                            img = s.Img,
                            name = s.Name
                        });
                    }
                    foreach (var s in companies[i].PopDestinaations)
                    {
                        di.Add(new DestinationListingInfoModel()
                        {
                            id = s.Id,
                            description = s.destination.Description,
                            img = s.destination.Img
                        ,
                            name = s.destination.Name
                        });
                    }
                    foreach (var s in companies[i].Raters)
                    {
                        ri.Add(new RaterListingInfoModel() { id = s.Id, rate = s.Rate });
                    }
                    foreach (var s in companies[i].FastTickets)
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

                    retCompanies = (new AirlineListingInfoModel()
                    {
                        Id = companies[i].Id,
                        Img = companies[i].Img,
                        MainLocation = companies[i].Address,
                        Name = companies[i].Name,
                        Rating = companies[i].Rating,
                        Lat = companies[i]
                    .Lat,
                        Lon = companies[i].Lon,
                        Admin = companies[i].Admin,
                        Description = companies[i].Description,
                        Raters = ri,
                        Destinations = di2,
                        PopDestinaations = di,
                        FastTickets = ti,
                        Flights = fl
                    });
                }
            }

            return retCompanies;
        }

    }
}
