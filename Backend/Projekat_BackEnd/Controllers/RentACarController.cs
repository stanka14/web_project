using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Projekat_BackEnd.Models.ContextData;
using Projekat_BackEnd.Models.Model;

namespace Projekat_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentACarController : Controller
    {
        private ApplicationUsersContext _context;
        public IConfiguration Configuration { get; }

        public RentACarController(ApplicationUsersContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        [HttpGet]
        [Route("GetAllActivatedCompanies")]
        public IEnumerable<RentACarListingInfoModel> GetAllActivatedCompanies()
        {
            var options = new DbContextOptionsBuilder<ApplicationUsersContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            List<RentACarListingInfoModel> retCompanies = new List<RentACarListingInfoModel>();
            using (var context = new ApplicationUsersContext(options))
            {
                var comps = context.RentACarCompanies.Include(comp => comp.MainLocation).ToList();

                foreach (var comp in comps)
                {
                    if (comp.Activated)
                    {
                        retCompanies.Add(new RentACarListingInfoModel() { Id = comp.ID, Img = comp.Image, MainLocation = comp.MainLocation.FullAddress, Name = comp.Name, Rating = comp.AvrageRating });
                    }
                }
            }
            return retCompanies;
        }

        [HttpPost]
        [Route("SearchCompanies")]
        public IEnumerable<RentACarListingInfoModel> SearchCompanies(SearchCompaniesModel searchModel)
        {
            var options = new DbContextOptionsBuilder<ApplicationUsersContext>()
                .UseSqlServer(Configuration.GetConnectionString("IdentityConnection")).Options;

            List<RentACarListingInfoModel> retCompanies = new List<RentACarListingInfoModel>();
            using (var context = new ApplicationUsersContext(options))
            {
                var companies = _context.RentACarCompanies
                .Include(comp => comp.MainLocation)
                .Include(comp => comp.Locations)
                .Include(comp => comp.Cars)
                    .ThenInclude(car => car.RentedDates)
                .Include(comp => comp.Cars)
                    .ThenInclude(car => car.Ratings)
                .Include(comp => comp.Ratings)
                .Include(comp => comp.QuickReservations)
                    .ThenInclude(qr => qr.Dates)
                .Include(comp => comp.extras).ToList();

                for (int i = 0; i < companies.Count; i++)
                {
                    if (companies[i].Name.ToLower() == searchModel.companyName.ToLower() || searchModel.companyName == "" || companies[i].Name.ToLower().Contains(searchModel.companyName.ToLower()))
                    {
                        if (searchModel.location == "")
                        {
                            bool available = false;
                            if (searchModel.from == "")
                                available = true;
                            else
                            {
                                foreach (var car in companies[i].Cars)
                                {
                                    if (!car.Removed && CheckAvailability(car, searchModel.from, searchModel.to))
                                        available = true;
                                }
                            }
                            if (available)
                                retCompanies.Add(new RentACarListingInfoModel() { Id = companies[i].ID, Img = companies[i].Image, MainLocation = companies[i].MainLocation.FullAddress, Name = companies[i].Name, Rating = companies[i].AvrageRating });
                        }
                        else
                        {
                            int found = 0;
                            string address = "";
                            for (int j = 0; j < companies[i].Locations.Count; j++)
                            {
                                if (companies[i].Locations[j].FullAddress.ToLower().Contains(searchModel.location.ToLower()))
                                {
                                    found = 1;
                                    address = companies[i].Locations[j].FullAddress;
                                    break;
                                }
                            }
                            if (found != 0)
                            {
                                bool available = false;
                                if (searchModel.from == "")
                                    available = true;
                                else
                                {
                                    foreach (var car in companies[i].Cars)
                                    {
                                        if (!car.Removed && CheckAvailability(car, searchModel.from, searchModel.to))
                                            available = true;
                                    }
                                }

                                if (available)
                                    retCompanies.Add(new RentACarListingInfoModel() { Id = companies[i].ID, Img = companies[i].Image, MainLocation = address, Name = companies[i].Name, Rating = companies[i].AvrageRating });
                            }
                        }

                    }
                }
            }

            return retCompanies;
        }

        [HttpPost]
        [Route("GetCompanyProfile")]
        public RentACarProfileInfoModel GetCompanyProfile(IdModel model)
        {
            RentACarProfileInfoModel company = new RentACarProfileInfoModel();
            var companies = _context.RentACarCompanies
                .Include(comp => comp.MainLocation)
                .Include(comp => comp.Locations)
                .Include(comp => comp.Ratings)
                .Include(comp => comp.extras).ToList();
            //int Id;
            //if (!Int32.TryParse(id, out Id))
            //    return null;

            foreach (var comp in companies)
            {
                if (comp.ID == model.id)
                {
                    company.Id = model.id;
                    company.Name = comp.Name;
                    company.Img = comp.Image;
                    company.Locations = comp.Locations;
                    company.MainLocation = comp.MainLocation;
                    company.Rating = comp.AvrageRating;
                    company.Description = comp.Description;
                    company.Extras = comp.extras;
                    return company;
                }

            }

            return null;
        }

        [HttpPost]
        [Route("SearchCars")]
        public List<CarModel> SearchCars(SearchCarModel searchModel)
        {
            List<CarModel> ret = new List<CarModel>();
            var companies = _context.RentACarCompanies
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

            var comp = _context.RentACarCompanies.Find(searchModel.companyId);
            if (comp == null)
                return ret;

            foreach (var car in comp.Cars)
            {
                if (!car.Removed)
                {
                    if (car.Location.ToLower().Contains(searchModel.location.ToLower().Trim()) || searchModel.location == "")
                    {
                        if (CheckBranch(comp, searchModel.returnLocation) || searchModel.returnLocation == "")
                        {
                            if (CheckAvailability(car, searchModel.dateFrom, searchModel.dateTo))
                            {
                                if (CheckQuickReservations(comp, car, searchModel.dateFrom, searchModel.dateTo))
                                {
                                    if (car.Year.ToString() == searchModel.year || searchModel.year == "")
                                    {
                                        if (car.Type.ToLower() == searchModel.type.ToLower() || searchModel.type == "")
                                        {
                                            if (car.Brand.ToLower() == searchModel.brand.ToLower() || searchModel.brand == "")
                                            {
                                                if (car.Model.ToLower() == searchModel.model.ToLower() || searchModel.model == "")
                                                {
                                                    if(searchModel.price != "")
                                                    {
                                                        try
                                                        {
                                                            if (car.PricePerDay <= Int32.Parse(searchModel.price))
                                                                ret.Add(new CarModel() { avrageRating = car.AvrageRating, brand = car.Brand, id = car.ID, image = car.Image, location = car.Location, model = car.Model, passengers = car.Passengers, price = GetTotalPrice(car, searchModel.dateFrom, searchModel.dateTo), type = car.Type, year = car.Year });
                                                        }
                                                        catch (Exception e)
                                                        {

                                                        }
                                                    }

                                                    ret.Add(new CarModel() { avrageRating = car.AvrageRating, brand = car.Brand, id = car.ID, image = car.Image, location = car.Location, model = car.Model, passengers = car.Passengers, price = GetTotalPrice(car, searchModel.dateFrom, searchModel.dateTo), type = car.Type, year = car.Year });
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
            return ret;
        }

        [HttpPost]
        [Route("GetCarInfo")]
        public CarModel GetCarInfo(IdModelCarComp idModelCarComp)
        {
            CarModel car = new CarModel();
            var companies = _context.RentACarCompanies.Include(comp => comp.Cars).ToList();
            foreach (var comp in companies)
            {
                if (comp.ID == idModelCarComp.idComp)
                {
                    foreach (var carr in comp.Cars)
                    {
                        if (carr.ID == idModelCarComp.idCar)
                        {
                            car.avrageRating = carr.AvrageRating;
                            car.brand = carr.Brand;
                            car.id = carr.ID;
                            car.image = carr.Image;
                            car.location = carr.Location;
                            car.model = carr.Model;
                            car.passengers = carr.Passengers;
                            car.price = carr.PricePerDay;
                            car.type = carr.Type;
                            car.year = carr.Year;
                            break;
                        }
                    }
                }
            }

            return car;
        }

        [HttpPost]
        [Route("SearchCarsHome")]
        public List<CarModel> SearchCarsHome(SearchCarModelHome searchModel)
        {
            List<CarModel> ret = new List<CarModel>();
            var companies = _context.RentACarCompanies
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

            foreach (var comp in companies)
            {
                foreach (var car in comp.Cars)
                {
                    if (!car.Removed)
                    {
                        if (car.Location.ToLower().Contains(searchModel.location.ToLower().Trim()) || searchModel.location == "")
                        {
                            if (CheckBranch(comp, searchModel.returnLocation) || searchModel.returnLocation == "")
                            {
                                if (CheckAvailability(car, searchModel.dateFrom, searchModel.dateTo))
                                {
                                    if (car.Type.ToLower() == searchModel.type.ToLower() || searchModel.type == "")
                                    {
                                        ret.Add(new CarModel() { avrageRating = car.AvrageRating, brand = car.Brand, id = car.ID, image = car.Image, location = car.Location, model = car.Model, passengers = car.Passengers, price = GetTotalPrice(car, searchModel.dateFrom, searchModel.dateTo), type = car.Type, year = car.Year, companyId = comp.ID });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //Thread.Sleep(5000);
            return ret;
        }

        [HttpGet]
        [Route("GetCompanyAmenities")]
        public List<ExtraAmenityModel> GetCompanyAmenities(int id)
        {
            List<ExtraAmenityModel> ret = new List<ExtraAmenityModel>();

            var companies = _context.RentACarCompanies
                .Include(comp => comp.extras)
                .ToList();

            foreach (var comp in companies)
            {
                if (comp.ID == id)
                {
                    foreach (var am in comp.extras)
                    {
                        ret.Add(new ExtraAmenityModel() { Id = am.Id, Image = am.Image, Name = am.Name, OneTimePayment = am.OneTimePayment, Price = am.Price, Selected = false });
                    }

                }
            }
            return ret;
        }

        [HttpGet]
        [Route("GetCompanyInfoAdmin")]
        public CompanyInfoModel GetCompanyInfoAdmin(int id)
        {
            CompanyInfoModel model = new CompanyInfoModel();

            var companies = _context.RentACarCompanies.ToList();

            foreach (var comp in companies)
            {
                if (comp.ID == id)
                {
                    model.name = comp.Name;
                    model.description = comp.Description;
                    model.logo = comp.Image;
                    return model;
                }
            }
            return null;
        }

        [HttpPost]
        [Route("GetDiscountedCars")]
        public List<CarModel> GetDiscountedCars(PlusRentModel model)
        {
            var companies = _context.RentACarCompanies
                .Include(comp => comp.MainLocation)
                .Include(comp => comp.Locations)
                .Include(comp => comp.Cars).ThenInclude(car => car.RentedDates)
                .Include(comp => comp.QuickReservations).ThenInclude(res => res.Dates)
                .Include(comp => comp.QuickReservations).ThenInclude(res => res.DiscountedCar)
                .Include(comp => comp.QuickReservations).ThenInclude(res => res.DiscountedCar).ThenInclude(car => car.RentedDates)
                .ToList();

            List<DateTime> datumi = GetDateTimeList(model.from, model.to);
            List<CarModel> cars = new List<CarModel>();

            foreach (var comp in companies)
            {
                if (CheckBranch(comp, model.location) && CheckBranch(comp, model.returnLocation))
                {
                    foreach (var res in comp.QuickReservations)
                    {
                        if (res.DiscountedCar.Location.ToLower().Trim().Contains(model.location.Trim().ToLower()))
                        {
                            if (!res.DiscountedCar.Removed)
                            {
                                List<DateTime> datumiRes = GetDateTimeList(res.Dates[0].DateStr, res.Dates[res.Dates.Count - 1].DateStr);
                                bool okay = true;
                                foreach (var d in datumi)
                                {
                                    if (!datumiRes.Contains(d))
                                        okay = false;
                                }
                                if (okay)
                                {
                                    if (CheckAvailability(res.DiscountedCar, res.Dates[0].DateStr, res.Dates[res.Dates.Count - 1].DateStr))
                                    {
                                        cars.Add(new CarModel() { avrageRating = res.DiscountedCar.AvrageRating, brand = res.DiscountedCar.Brand, id = res.DiscountedCar.ID, image = res.DiscountedCar.Image, location = res.DiscountedCar.Location, model = res.DiscountedCar.Model, passengers = res.DiscountedCar.Passengers, price = res.DiscountedCar.PricePerDay * datumi.Count, type = res.DiscountedCar.Type, year = res.DiscountedCar.Year });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return cars;
        }

        [HttpGet]
        [Route("GetDiscountedCarsForCompany")]
        public List<CarModel> GetDiscountedCarsForCompany(int id)
        {
            var companies = _context.RentACarCompanies
                .Include(comp => comp.MainLocation)
                .Include(comp => comp.Locations)
                .Include(comp => comp.Cars).ThenInclude(car => car.RentedDates)
                .Include(comp => comp.QuickReservations).ThenInclude(res => res.Dates)
                .Include(comp => comp.QuickReservations).ThenInclude(res => res.DiscountedCar)
                .Include(comp => comp.QuickReservations).ThenInclude(res => res.DiscountedCar).ThenInclude(car => car.RentedDates)
                .ToList();

            List<CarModel> cars = new List<CarModel>();

            var company = _context.RentACarCompanies.Find(id);

            foreach(var qr in company.QuickReservations)
            {
                if(cars.Find(car=>car.id == qr.DiscountedCar.ID) == null)
                {
                    cars.Add(new CarModel() { avrageRating = 0, brand = qr.DiscountedCar.Brand, id = qr.DiscountedCar.ID, image = qr.DiscountedCar.Image, location = qr.DiscountedCar.Location, model = qr.DiscountedCar.Model, passengers = qr.DiscountedCar.Passengers, price = qr.DiscountedCar.PricePerDay, type = qr.DiscountedCar.Type, year = qr.DiscountedCar.Year });
                }
            }
            return cars;
        }

        private List<Date> GetDates (string from, string to)
        {
            List<Date> dates = new List<Date>();

            DateTime fromDate = DateTime.Parse(from);
            DateTime toDate = DateTime.Parse(to);

            dates.Add(new Date() { DateStr = fromDate.ToShortDateString() });

            while(fromDate < toDate)
            {
                DateTime d = fromDate.AddDays(1);
                dates.Add(new Date() { DateStr = d.ToShortDateString() });
            }

            return dates;
        }

        private bool CheckAvailability(Car car, string from, string to)
        {
            bool available = true;
            DateTime fromDate = DateTime.Parse(from); 
            DateTime toDate = DateTime.Parse(to);

            foreach (var date in car.RentedDates)
            {
                DateTime dt = DateTime.Parse(date.DateStr);
                if(dt >= fromDate && dt <= toDate)
                {
                    available = false;
                    break;
                }
            }

            return available;
        }

        private bool CheckBranch(RentACar company, string location)
        {
            
            if (company.MainLocation.FullAddress.ToLower().Trim().Contains(location.Trim().ToLower()))
                return true;
            bool exists = false;
            foreach (var loc in company.Locations)
            {
                if (loc.FullAddress.ToLower().Trim().Contains(location.Trim().ToLower()))
                {
                    exists = true;
                    break;
                }
            }

            return exists;
        }

        private double GetTotalPrice(Car car, string from, string to )
        {
            DateTime dtfrom = DateTime.Parse(from);
            DateTime dtTo = DateTime.Parse(to);
            double days = (dtTo - dtfrom).TotalDays;
            return car.PricePerDay * days;

        }

        private bool CheckQuickReservations(RentACar comp, Car car, string from, string to)
        {
            bool available = true;
            DateTime fromDate = DateTime.Parse(from);
            DateTime toDate = DateTime.Parse(to);

            List<DateTime> dates = GetDateTimeList(from, to);

            foreach (var res in comp.QuickReservations)
            {
                if (res.DiscountedCar.ID == car.ID)
                {
                    foreach (var d in dates)
                    {
                        List<DateTime> pom = GetDateTimeList(res.Dates[0].DateStr, res.Dates[res.Dates.Count - 1].DateStr);
                        if (pom.Contains(d))
                        {
                            return false;
                        }
                    }
                }
            }

            return available;
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
    }
}