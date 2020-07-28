﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Projekat_BackEnd.Models.Model
{
    public class User : IdentityUser
    {
        [Column(TypeName = "nvarchar(150)")]
        public string FullName { get; set; }
        public string Address { get; set; }
        public DateTime Birthday { get; set; }
        public string Passport { get; set; }
        public List<Ticket> Flights { get; set; }
        public List<TicketInvitation> FlightRequests { get; set; }
        public List<CarReservation> RentedCars { get; set; }
        public List<User> Friends { get; set; }
        public List<FriendRequestReceived> FriendRequests { get; set; }
        public List<FriendRequestSent> SentRequests { get; set; }
        public List<Notification> Notifications { get; set; }
        public int Points { get; set; }
        public int CompanyId { get; set; }
        public bool ChangedPassword { get; set; }
        public bool MainWebsiteAdministrator { get; set; }
        public Discount Discount { get; set; }
        public bool SocialUser { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }

    }

    public class Discount
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public double BronzeTier { get; set; }
        public double SilverTier { get; set; }
        public double GoldTier { get; set; }
        public double DiscountPercent { get; set; }

    }

    public class DiscountModel
    {
        public double bronzeTier { get; set; }
        public double silverTier { get; set; }
        public double goldTier { get; set; }
        public double discountPercent { get; set; }
    }

    public class UserModel
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Birthday { get; set; }
        public string Passport { get; set; }
        public List<TicketListingInfoModel> Flights { get; set; }
        public List<TicketListingInfoModel> OldFlights { get; set; }
        public List<TicketListingInfoModel> FlightRequests { get; set; }
        public List<UserModel> Friends { get; set; }
        public List<FriendRequestReceivedModel> FriendRequests { get; set; }
        public List<FriendRequestSentModel> SentRequests { get; set; }
        public List<Notification> Notifications { get; set; }
        public List<CarReservationModel> RentedCars { get; set; }
        public int Points { get; set; }
        public int CompanyId { get; set; }

        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool changedPassword { get; set; }
        public bool socialUser { get; set; }

    }

    public class CarRentalAdminModel
    {
        public string fullName { get; set; }
        public string address { get; set; }
        public string birthday { get; set; }
        public int companyId { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public bool verifiedEmail { get; set; }
        public bool changedPassword { get; set; }
        public CompanyInfoModel companyInfo { get; set; }

        public CarRentalAdminModel() { }

    }

    public class WebsiteAdminModel
    {
        public string fullName { get; set; }
        public string address { get; set; }
        public string birthday { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public bool verifiedEmail { get; set; }
        public bool changedPassword { get; set; }
        public bool mainAdmin { get; set; }
        public List<OtherAdmin> websiteAdministrators { get; set; }
        public List<CarCompanyModel> rentAcars { get; set; }
        public List<AirCompanyModel> airlines { get; set; }
        public DiscountModel discount { get; set; }

        public WebsiteAdminModel() { }

    }

    public class OtherAdmin
    {
        public string fullname { get; set; }
        public string username { get; set; }
        public string email { get; set; }

        public OtherAdmin() { }
    }

    public class NewWebAdminModel
    {
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }

        public NewWebAdminModel() { }
    }
    public class Notification
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Text { get; set; }

        public Notification() { }
    }

    public class LocationsModel
    {
        public Address mainLocation { get; set; }
        public List<Address> locations { get; set; }

        public LocationsModel() { }
    }

    public class FriendRequestSent
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string User { get; set; }

        public FriendRequestSent() { }
    }

    public class FriendRequestReceived
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string User { get; set; }

        public FriendRequestReceived() { }
    }
    public class FriendRequestSentModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public UserModel User { get; set; }

        public FriendRequestSentModel() { }
    }

    public class FriendRequestReceivedModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public UserModel User { get; set; }

        public FriendRequestReceivedModel() { }
    }
    public class TicketInvitation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Ticket Ticket { get; set; }

        public TicketInvitation() { }
    }
    
    public class ChangePasswordModel
    {
        public string newPassword { get; set; }
        public string password { get; set; }
        public ChangePasswordModel() { }
    }
    public class ChangeAccountModel
    {
        public string email { get; set; }
        public string fullName { get; set; }
        public string address { get; set; }
        public ChangeAccountModel() { }
    }
    
    public class SearchUserModel
    {
        public string passport { get; set; }
        public SearchUserModel() { }
    }

    public class RemoveReservationModel
    {
        public int resId { get; set; }
        public int compId { get; set; }

        public RemoveReservationModel() { }
    }

    public class AdminDetailsModel
    {
        public string name { get; set; }
        public string bd { get; set; }
        public string addr { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        

        public AdminDetailsModel() { }
    }

    public class AdminUsernameModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public AdminUsernameModel() { }
    }

    public class AdminPasswordModel
    {
        public string password { get; set; }
        public string newPassword { get; set; }
        public AdminPasswordModel() { }
    }

    public class AddAdminModel
    {
        public int companyId { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string password2 { get; set; }

        public AddAdminModel() { }
    }

    public class AirCompanyModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public List<string> admins { get; set; }

        public AirCompanyModel() { }
    }

    public class SendRequestModel
    { 
        public string idFrom { get; set; }
        public string idTo { get; set; }
        public SendRequestModel() { }
    }
    public class RateFlightModel
    {
        public int id { get; set; }
        public int flight { get; set; }

        public RateFlightModel() { }
    }
    public class RateCompanyModel
    {
        public int id { get; set; }
        public int flightiD { get; set; }

        public string compId { get; set; }
        public RateCompanyModel() { }
    }
}
