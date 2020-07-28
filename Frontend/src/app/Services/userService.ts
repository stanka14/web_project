import { Injectable } from '@angular/core';
import { Time } from '@angular/common';
import { User } from 'src/app/entities/user/user';
import { Flight } from '../entities/flight/fligh';
import { UserType } from '../entities/userType/userType';
import { Car } from '../entities/car/car';
import { CarReservation } from 'src/app/entities/carReservation/carReservation'
import { Ticket } from '../entities/ticket/ticket';
import { HttpClient } from '@angular/common/http';

@Injectable({
    providedIn: 'root',
   })

export class UserService {
    readonly BaseURI = 'http://localhost:57886/api';

    users = new Array<User>();
    user: User;
    user2: User;
    user3: User;
    user4: User;
    user5: User;
    user6: User;
    user7: User;
    user8: User;
    user9: User;
    user10: User;
    user11: User;
    user12: User;
    user13: User;
    user14: User;
    zahtjevi_pom: Array<User>;
    loggedUser: User;
    logged: boolean;
    userDates: Array<string>;
    userRetLocation: string;
    constructor(private http: HttpClient) {

        // tslint:disable-next-line: max-line-length
        this.user3 = {flightRequests: new Array<Ticket>(), oldFlights: new Array<Ticket>(), notifications: new Array<string>(), sentRequests: new Array<User>(), passport: 'A13KD1', birthday: new Date(2020, 4, 1, 22, 0, 0, 0), friendRequests: new Array<User>(), id: 2, firstName: 'Nina',
        lastName: 'Sarenac', email: 'sarenac@gmail.com',
        address: 'Zvornik', password: 'nina', username: 'nina',
        type: UserType.CarRentalAdmin, myFlights: new Array<Ticket>(), rentedCars: new Array<CarReservation>(), friends: new Array<User>(), points: 0};
        this.users.push(this.user3);

        this.zahtjevi_pom = [this.user3];

        // tslint:disable-next-line: max-line-length
        this.user = {flightRequests: new Array<Ticket>(), oldFlights: new Array<Ticket>(), notifications: new Array<string>(), sentRequests: new Array<User>(), passport: 'A13KD1', birthday: new Date(2020, 4, 1, 22, 0, 0, 0), friendRequests: this.zahtjevi_pom, id: 1, firstName: 'Stanka',
            lastName: 'Kosutic', email: 'stankic07@gmail.com',
            address: 'Srpskih ratnika bb', password: 'stanka',
         // tslint:disable-next-line: max-line-length
         username: 'stanka', type: UserType.AirlineAdmin, myFlights: new Array<Ticket>(), rentedCars: new Array<CarReservation>(), friends: new Array<User>(), points:0};
        this.users.push(this.user);

        this.logged = false;

        // tslint:disable-next-line: max-line-length
        this.user2 = {flightRequests: new Array<Ticket>(), oldFlights: new Array<Ticket>(), notifications: new Array<string>(), sentRequests: new Array<User>(), passport: 'A13KD1', birthday: new Date(2020, 4, 1, 22, 0, 0, 0), friendRequests: new Array<User>(), id: 3, firstName: 'Ivana',
        lastName: 'Kosutic', email: 'ivana@gmail.com',
        address: 'Srpskih ratnika bb', password: 'ivana',
        // tslint:disable-next-line: max-line-length
        username: 'ivana', type: UserType.Registered, myFlights: new Array<Ticket>(), rentedCars: new Array<CarReservation>(), friends: new Array<User>(), points:0};
        this.users.push(this.user2);

        this.user4 = {flightRequests: new Array<Ticket>(), oldFlights: new Array<Ticket>(), notifications: new Array<string>(), sentRequests: new Array<User>(), passport: 'A13BB1', birthday: new Date(2011, 10, 5, 0, 0, 0, 0), friendRequests: new Array<User>(), id: 4, firstName: 'Milica',
        lastName: 'Sarenac', email: 'milica@gmail.com',
        address: 'Svetog Save 68', password: 'milica',
        username: 'milica', type: UserType.Admin, myFlights: new Array<Ticket>(), rentedCars: new Array<CarReservation>(), friends: new Array<User>(), points:0};
        this.users.push(this.user4);

        this.user5 = {flightRequests: new Array<Ticket>(), oldFlights: new Array<Ticket>(), notifications: new Array<string>(), sentRequests: new Array<User>(), passport: 'A13BB1', birthday: new Date(2011, 10, 5, 0, 0, 0, 0), friendRequests: new Array<User>(), id: 5, firstName: 'Milica',
        lastName: 'Sarenac', email: 'milica@gmail.com',
        address: 'Svetog Save 68', password: 'bilja',
        username: 'bilja', type: UserType.CarRentalAdmin, myFlights: new Array<Ticket>(), rentedCars: new Array<CarReservation>(), friends: new Array<User>(), points:0};
        this.users.push(this.user5);

        this.user6 = {flightRequests: new Array<Ticket>(), oldFlights: new Array<Ticket>(), notifications: new Array<string>(), sentRequests: new Array<User>(), passport: 'A13BB1', birthday: new Date(2011, 10, 5, 0, 0, 0, 0), friendRequests: new Array<User>(), id: 6, firstName: 'Milica',
        lastName: 'Sarenac', email: 'milica@gmail.com',
        address: 'Svetog Save 68', password: 'ljilja',
        username: 'ljilja', type: UserType.CarRentalAdmin, myFlights: new Array<Ticket>(), rentedCars: new Array<CarReservation>(), friends: new Array<User>(), points:0};
        this.users.push(this.user6);

        this.user7 = {flightRequests: new Array<Ticket>(), oldFlights: new Array<Ticket>(), notifications: new Array<string>(), sentRequests: new Array<User>(), passport: 'A13BB1', birthday: new Date(2011, 10, 5, 0, 0, 0, 0), friendRequests: new Array<User>(), id: 7, firstName: 'Milica',
        lastName: 'Sarenac', email: 'milica@gmail.com',
        address: 'Svetog Save 68', password: 'toma',
        username: 'toma', type: UserType.CarRentalAdmin, myFlights: new Array<Ticket>(), rentedCars: new Array<CarReservation>(), friends: new Array<User>(), points:0};
        this.users.push(this.user7);

        this.user8 = {flightRequests: new Array<Ticket>(), oldFlights: new Array<Ticket>(), notifications: new Array<string>(), sentRequests: new Array<User>(), passport: 'A13BB1', birthday: new Date(2011, 10, 5, 0, 0, 0, 0), friendRequests: new Array<User>(), id: 8, firstName: 'Milica',
        lastName: 'Sarenac', email: 'milica@gmail.com',
        address: 'Svetog Save 68', password: 'steva',
        username: 'steva', type: UserType.CarRentalAdmin, myFlights: new Array<Ticket>(), rentedCars: new Array<CarReservation>(), friends: new Array<User>(), points:0};
        this.users.push(this.user8);

        this.user9 = {flightRequests: new Array<Ticket>(), oldFlights: new Array<Ticket>(), notifications: new Array<string>(), sentRequests: new Array<User>(), passport: 'A13BB1', birthday: new Date(2011, 10, 5, 0, 0, 0, 0), friendRequests: new Array<User>(), id: 9, firstName: 'Milica',
        lastName: 'Sarenac', email: 'milica@gmail.com',
        address: 'Svetog Save 68', password: 'zika',
        username: 'zika', type: UserType.CarRentalAdmin, myFlights: new Array<Ticket>(), rentedCars: new Array<CarReservation>(), friends: new Array<User>(), points:0};
        this.users.push(this.user9);

        this.user10 = {flightRequests: new Array<Ticket>(), oldFlights: new Array<Ticket>(), notifications: new Array<string>(), sentRequests: new Array<User>(), passport: 'A13BB1', birthday: new Date(2011, 10, 5, 0, 0, 0, 0), friendRequests: new Array<User>(), id: 10, firstName: 'Milica',
        lastName: 'Sarenac', email: 'milica@gmail.com',
        address: 'Svetog Save 68', password: 'mika',
        username: 'mika', type: UserType.CarRentalAdmin, myFlights: new Array<Ticket>(), rentedCars: new Array<CarReservation>(), friends: new Array<User>(), points:0};
        this.users.push(this.user10);

        this.user11 = {flightRequests: new Array<Ticket>(), oldFlights: new Array<Ticket>(), notifications: new Array<string>(), sentRequests: new Array<User>(), passport: 'A13BB1', birthday: new Date(2011, 10, 5, 0, 0, 0, 0), friendRequests: new Array<User>(), id: 11, firstName: 'Milica',
        lastName: 'Sarenac', email: 'milica@gmail.com',
        address: 'Svetog Save 68', password: 'pera',
        username: 'pera', type: UserType.CarRentalAdmin, myFlights: new Array<Ticket>(), rentedCars: new Array<CarReservation>(), friends: new Array<User>(), points:0};
        this.users.push(this.user11);

        this.user12 = {flightRequests: new Array<Ticket>(), oldFlights: new Array<Ticket>(), notifications: new Array<string>(), sentRequests: new Array<User>(), passport: 'A13BB1', birthday: new Date(2011, 10, 5, 0, 0, 0, 0), friendRequests: new Array<User>(), id: 12, firstName: 'Milica',
        lastName: 'Sarenac', email: 'milica@gmail.com',
        address: 'Svetog Save 68', password: 'sanja',
        username: 'sanja', type: UserType.AirlineAdmin, myFlights: new Array<Ticket>(), rentedCars: new Array<CarReservation>(), friends: new Array<User>(), points:0};
        this.users.push(this.user12);

        this.user13 = {flightRequests: new Array<Ticket>(), oldFlights: new Array<Ticket>(), notifications: new Array<string>(), sentRequests: new Array<User>(), passport: 'A13BB1', birthday: new Date(2011, 10, 5, 0, 0, 0, 0), friendRequests: new Array<User>(), id: 13, firstName: 'Milica',
        lastName: 'Sarenac', email: 'milica@gmail.com',
        address: 'Svetog Save 68', password: 'ceca',
        username: 'ceca', type: UserType.AirlineAdmin, myFlights: new Array<Ticket>(), rentedCars: new Array<CarReservation>(), friends: new Array<User>(), points:0};
        this.users.push(this.user13);

        this.user14 = {flightRequests: new Array<Ticket>(), oldFlights: new Array<Ticket>(), notifications: new Array<string>(), sentRequests: new Array<User>(), passport: 'A13BB1', birthday: new Date(2011, 10, 5, 0, 0, 0, 0), friendRequests: new Array<User>(), id: 14, firstName: 'Milica',
        lastName: 'Sarenac', email: 'milicamiki@gmail.com',
        address: 'Svetog Save 68', password: 'miki',
        username: 'miki', type: UserType.Registered, myFlights: new Array<Ticket>(), rentedCars: new Array<CarReservation>(), friends: new Array<User>(), points:0};
        this.users.push(this.user14);

        this.userDates = new Array<string>();
    }
    allUsers()
    {
        var users = new Array<User>();

        users = this.users.filter(item3 => item3 != this.loggedUser);

        return users;
    }
    allPeople(id: number)
    {
        var user = this.FindUser(id);
        var users = new Array<User>();
        users = this.users.filter(item3 => item3 != this.loggedUser);

        users.forEach(item =>
        {
            user.friends.forEach(friend =>
            {
                if (item.username == friend.username){
                    users = users.filter(item3 => item3 != item);
                }
            });
            user.sentRequests.forEach(req =>
            {
                if (req.username == item.username){
                    users = users.filter(item3 => item3 != item);
                }
            });
            user.friendRequests.forEach(fr =>
            {
                if (fr.username == item.username){
                    users = users.filter(item3 => item3 != item);
                }
            });
        });

        return users;
    }
    FindUser(id: number)
    {
        // tslint:disable-next-line: prefer-for-of
        for (let i = 0; i < this.users.length; i++){
            // tslint:disable-next-line: triple-equals
            if (this.users[i].id == id)
            {
                return this.users[i];
            }
        }
    }
    FriendRequest(id: number, user: User)
    {
        
        // tslint:disable-next-line: prefer-for-of
        for (let i = 0; i < this.users.length; i++){
            // tslint:disable-next-line: triple-equals
            if (this.users[i].id == id)
            {
                this.users[i].friendRequests.push(user);

                // tslint:disable-next-line: prefer-for-of
                for (let j = 0; j < this.users.length; j++){
                    // tslint:disable-next-line: triple-equals
                    if (this.users[j] == user)
                    {
                        this.users[j].sentRequests.push(this.users[i]);
                    }
                }
                return this.users[i];
            }
        }
    }
    FindUserByEmail(fn: string)
    {
        // tslint:disable-next-line: prefer-for-of
        for (let i = 0; i < this.users.length; i++){
            // tslint:disable-next-line: triple-equals
            if (this.users[i].email == fn)
            {
                return this.users[i];
            }
        }
    }
    addUser(user: User)
    {
        this.users.push(user);
        this.user = user;
        this.logged = true;
        return user.id;
    }
    isLoggedUser()
    {
        return this.loggedUser;
    }
    logIn(un: string, pass: string)
    {
        // tslint:disable-next-line: prefer-for-of
        for (let i = 0; i < this.users.length; i++){
            // tslint:disable-next-line: triple-equals
            if (this.users[i].username == un)
            {
                if (this.users[i].password == pass)
                {    
                    this.loggedUser = this.users[i];
                    this.logged = true;
                    return true;
                }
            }
        }
        return false;
    }
    addFlight(flight: Ticket)
    {

        // tslint:disable-next-line: prefer-for-of
        for (let i = 0; i < this.users.length; i++){
            // tslint:disable-next-line: triple-equals
            if (this.users[i].username == this.loggedUser.username)
            {
                this.users[i].myFlights.push(flight);
                this.loggedUser = this.users[i];
            }
        }
    }
    removeFlight(idF: number)
    {

        // tslint:disable-next-line: prefer-for-of
        for (let i = 0; i < this.users.length; i++){
            // tslint:disable-next-line: triple-equals
            if (this.users[i].username == this.loggedUser.username)
            {
                // tslint:disable-next-line: prefer-for-of
                for (let j = 0; j < this.users[i].myFlights.length; j++){
                    // tslint:disable-next-line: triple-equals
                    if (this.users[i].myFlights[j].id == idF)
                    {
                        const index = this.users[i].myFlights.indexOf(this.users[i].myFlights[j]);                 
                        this.users[i].myFlights.splice(index, 1);
                        this.loggedUser = this.users[i];
                    }
                }
            }
        }
    }
    // addCar(company: RentAcar, car: Car, user:User, dates:Array<string>, pLoc:string, pTime:string, dLoc:string, dTime:string) {
    //     for (let i = 0; i < this.users.length; i++){
    //         if (this.users[i].username == this.loggedUser.username)
    //         {
    //             this.users[i].rentedCars.push(new CarReservation(car, car.brand + ' ' + car.model, user, dates, company.id, company.name, dates[0], dates[dates.length - 1], car.pricePerDay*dates.length, 0, 0, pLoc, pTime, dLoc, dTime, new Date(Date.now())));
    //             this.loggedUser = this.users[i];
    //         }
    //     }
    // }
    removeCar(carID: number) {
        for (let i = 0; i < this.users.length; i++){
            if (this.users[i].username == this.loggedUser.username)
            {
                for (let j = 0; j < this.users[i].rentedCars.length; j++){
                    if (this.users[i].rentedCars[j].carEn.id == carID)
                    {
                        const index = this.users[i].rentedCars.indexOf(this.users[i].rentedCars[j]);                 
                        this.users[i].rentedCars.splice(index, 1);
                        this.loggedUser = this.users[i];
                    }
                }
            }
        }
    }
    isAdmin()
    {
        if (this.logged == true)
        {
            if (this.loggedUser.type == UserType.Admin)
            {
                return true;
            }
        }
        return false;
    }
    isRegistered()
    {
        if (this.logged == true)
        {
            if (this.loggedUser.type == UserType.Registered)
            {
                return true;
            }
        }
        return false;
    }
    GetUserType(username: string) {
        for (let i = 0; i < this.users.length; i++){
            if (this.users[i].username == username)
            return this.users[i].type;
        }
    }
    logOut()
    {
        this.logged = false;
    }
    usernameExists(username: string) {
        for (let i = 0; i < this.users.length; i++){
            if (this.users[i].username == username)
                return true;
        }
        return false;
    }

   

    ChangeUsername(id:number, newUn:string) {
        for (let i = 0; i < this.users.length; i++){
            if (this.users[i].id == id) {
                this.users[i].username = newUn;
            }
        }
    }

    saveDates(dates: Array<string>) {
        this.userDates = dates;
    }

    getUserDates() {
        return this.userDates;
    }

    saveRetLocation(retLoc: string) {
        this.userRetLocation = retLoc;
    }

    getUserRetLocation() {
        return this.userRetLocation;
    }

    addPoints(idUser: number, points: number) {
        for(let i = 0; i<this.users.length; i++) {
            if(this.users[i].id == idUser) {
                this.users[i].points = this.users[i].points + points;
                return this.users[i];
            }
        }
    }

    getAllWebsiteAdmins() {
        var admins = new Array<User>();
        for(let i = 0; i<this.users.length; i++) {
            if(this.users[i].type == UserType.Admin) {
                admins.push(this.users[i]);
            }
        }
        return admins;
    }

    addAdministrator(email: string, firstName:string, lastName:string, pass:string, address:string, phone:string, birthday:Date, passport:string, username:string){
        this.users.push(new User(new Array<Ticket>(), 
        new Array<Ticket>(), new Array<string>(), new Array<User>(), passport, birthday, new Array<User>(), 100+this.users.length, firstName, lastName, address, username, pass, email, UserType.Admin, new Array<Ticket>(), new Array<CarReservation>(), 0));
        return this.getAllWebsiteAdmins();
    }

    getAllCarCompanyAdmins() {
        var admins = new Array<User>();
        for(let i = 0; i<this.users.length; i++) {
            if(this.users[i].type == UserType.CarRentalAdmin) {
                admins.push(this.users[i]);
            }
        }
        return admins;
    }

    getAllAirlineAdmins() {
        var admins = new Array<User>();
        for(let i = 0; i<this.users.length; i++) {
            if(this.users[i].type == UserType.AirlineAdmin) {
                admins.push(this.users[i]);
            }
        }
        return admins;
    }

    reservationsMade(companyId: number) {
        for(let i = 0; i < this.users.length; i++) {
            for(let j = 0; j<this.users[i].rentedCars.length; j++) {
                
            }
        }
    }

    isLoggedIn():boolean {
        return this.logged;
    }

    getCurrentRole() {
        return this.loggedUser.type.toString();
    }

    login(formData) {
        return this.http.post(this.BaseURI + '/AppUser/Login', formData);
    }

    register(formData1) {
        return this.http.post(this.BaseURI + '/AppUser/Register', formData1);
    }
    loadUser() {
        return this.http.get(this.BaseURI + '/AppUser/LoadUser');
    }
    loadFlightRequests() {
        return this.http.get(this.BaseURI + '/AppUser/LoadFlightRequests');
    }
    loadUserById(id) {
        return this.http.post(this.BaseURI + '/AppUser/LoadUserById', id);
    }   
    CancelFlightRequest(model) {
        return this.http.post(this.BaseURI + '/AppUser/CancelFlightRequest', model);
    }
    CancelFlight(model) {
        return this.http.post(this.BaseURI + '/AppUser/CancelFlight', model);
    }
    AcceptFlightRequest(model) {
        return this.http.post(this.BaseURI + '/AppUser/AcceptFlightRequest', model);
    }
    loadPeople() {
        return this.http.get(this.BaseURI + '/AppUser/loadPeople');
    }
    sendRequest(model) {
        return this.http.post(this.BaseURI + '/AppUser/SendRequest', model);
    }
    findUserByPassport(sm) {
        return this.http.post(this.BaseURI + '/AppUser/SearchUserByPassport',sm);
    }
    finish(sm) {
        return this.http.post(this.BaseURI + '/AppUser/Finish',sm);
    }
    getAllCompanies() {
        return this.http.get(this.BaseURI + '/Airline/GetAllActivatedCompanies');
    }
    acceptRequest(model) {
        return this.http.post(this.BaseURI + '/AppUser/AcceptRequest', model);
    }

    CancelRequest(model) {
        return this.http.post(this.BaseURI + '/AppUser/CancelRequest', model);
    }
    RemoveFriend(model) {
        return this.http.post(this.BaseURI + '/AppUser/RemoveFriend', model);
    }
    CheckUsername(model) {
        return this.http.post(this.BaseURI + '/AppUser/CheckUsername', model);
    }
    CheckPassword(model) {
        return this.http.post(this.BaseURI + '/AppUser/CheckPassword', model);
    }
    ChangeUserName(model) {
        return this.http.post(this.BaseURI + '/AppUser/ChangeUserName', model);
    }
    loadFlightById(idModel) {

        return this.http.post(this.BaseURI + '/AppUser/LoadFlight', idModel);
    }
    ChangePassword(model) {
        return this.http.post(this.BaseURI + '/AppUser/ChangePassword', model);
    }
    SaveNewAccountDetails(model) {
        return this.http.post(this.BaseURI + '/AppUser/SaveNewAccountDetails', model);
    }   
     IsAdmin(model) {
        return this.http.post(this.BaseURI + '/AppUser/IsAdmin', model);
    }
    DisableSeat(model) {
        return this.http.post(this.BaseURI + '/AppUser/DisableSeat', model);
    }
    FastReserve(model) {
        return this.http.post(this.BaseURI + '/AppUser/FastReserve', model);
    }
    RateFlight(model) {
        return this.http.post(this.BaseURI + '/AppUser/RateFlight', model);
    }
    RateCompany(model) {
        return this.http.post(this.BaseURI + '/AppUser/RateCompany', model);
    }
    loadCarRentalAdmin() {
        return this.http.get(this.BaseURI + '/AppUser/LoadCarRentalAdmin');
    }
    loadCompanyForAdmin() {
        return this.http.get(this.BaseURI + '/AppUser/GetCompanyForAdmin');
    }
    SaveAdminAccountDetails(name:string, bd:string, addr:string, pass:string, email:string){
        var model = {
            name : name,
            bd: bd.toString(),
            addr: addr,
            password: pass,
            email: email
        }
        return this.http.post(this.BaseURI + '/AppUser/ChangeAdminAccountDetails', model);
    }
    SaveNewUsernameAdmin(username:string, pass:string) {
        var model = {
            username: username,
            password: pass
        }
        return this.http.post(this.BaseURI + '/AppUser/ChangeAdminUsername', model);
    }
    SaveNewPasswordAdmin(pass:string, newPass:string){
        var model = {
            password: pass,
            newPassword: newPass
        }
        return this.http.post(this.BaseURI + '/AppUser/ChangeAdminPassword', model);
    }
    loadWebsiteAdmin() {
        return this.http.get(this.BaseURI + '/AppUser/LoadWebsiteAdmin');
    }
    SaveWAdminAccountDetails(name:string, bd:string, addr:string, pass:string, email:string) {
        var model = {
            name : name,
            bd: bd.toString(),
            addr: addr,
            password: pass,
            email: email
        }
        return this.http.post(this.BaseURI + '/AppUser/ChangeWAdminAccountDetails', model);
    }
    SaveNewUsernameWAdmin(username:string, pass:string) {
        var model = {
            username: username,
            password: pass
        }
        return this.http.post(this.BaseURI + '/AppUser/ChangeWAdminUsername', model);
    }
    SaveNewPasswordWAdmin(pass:string, newPass:string){
        var model = {
            password: pass,
            newPassword: newPass
        }
        return this.http.post(this.BaseURI + '/AppUser/ChangeWAdminPassword', model);
    }
    AddNewWebAdmin(model) {
        return this.http.post(this.BaseURI + '/AppUser/AddNewWebAdmin', model);
    }
    RegisterCarCompany(companyName, email, username, pass, clickedAddress, clickedLat, clickedLon) {
        var model = {
            companyName: companyName,
            address: clickedAddress,
            latitude: clickedLat,
            longitude: clickedLon,
            email:email,
            username:username,
            password:pass
        }
        return this.http.post(this.BaseURI + '/AppUser/RegisterCarCompany', model);

    }
    AddAdminToCompany(model) {
        return this.http.post(this.BaseURI + '/AppUser/AddAdminToCompany', model);
    }
    AddAdminToACompany(model) {
        return this.http.post(this.BaseURI + '/AppUser/AddAdminToAirCompany', model);
    }
    RegisterAirCompany(model) {
        return this.http.post(this.BaseURI + '/AppUser/RegisterAirCompany', model);
    }
    SaveDiscount(model){
        return this.http.post(this.BaseURI + '/AppUser/SaveDiscount', model);
    }
    GetProfitForAdmin(model){
        return this.http.post(this.BaseURI + '/AppUser/GetProfit', model);
    }
    socialLogIn(model){
        return this.http.post(this.BaseURI + '/AppUser/SocialLogIn', model);
    }
    socialLogInFacebook(model){
        return this.http.post(this.BaseURI + '/AppUser/SocialLogInFb', model);
    }
    AvailableCarsAdmin(model) {
        return this.http.post(this.BaseURI + '/AppUser/AvailableCarsAdmin', model);
    }
    
}
