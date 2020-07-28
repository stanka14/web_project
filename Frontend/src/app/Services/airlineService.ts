import { Airline } from 'src/app/entities/airline/airline';
import { Destination } from 'src/app/entities/destination/destination';
import { Injectable } from '@angular/core';
import { Flight } from 'src/app/entities/flight/fligh';
import { Time } from '@angular/common';
import { Seat } from '../entities/Seat/seat';
import { Classes } from '../entities/flight/class';
import { Trip } from '../entities/flight/trip';
import { User } from '../entities/user/user';
import { Traveller } from '../entities/user/traveller';
import { UserService } from './userService';
import { Luggage } from '../entities/flight/luggage';
import { Ticket } from '../entities/ticket/ticket';
import { Address } from '../entities/address/address';
import { HttpClient } from '@angular/common/http';


@Injectable({
    providedIn: 'root',
   })

export class AirlineService {
    a: Airline;
    d1: Destination = {id: 1, name: 'Belgrade', img: 'belgrade.jpg', description: 'The capital and largest city of Serbia. It is located at the confluence of the Sava and Danube'};
    d2: Destination = {id: 2, name: 'Paris', img: 'paris.jpg', description: 'Paris contains the most visited monuments in the city, including the Notre Dame Cathedral and the Louvre as well as the Sainte-Chapelle and the Eiffel Tower.'};
    d3: Destination = {id: 3, name: 'Banja Luka', img: 'banjaluka.jpg', description: 'The city lies on the Vrbas River and is well known in the countries of the former Yugoslavia for being full of tree-lined avenues, boulevards, gardens, and parks.'};
    d4: Destination = {id: 4, name: 'London', img: 'london.jpg', description: 'London is considered to be one of the  most important global cities and has been called the most powerful, most desirable, most influential and most visited city.'};

    u1: User;
    t1: Traveller;
    s1: Seat = {type: Classes.First, traveller: this.t1, id: 1, taken: true, isSelected: false};
    s2: Seat = {type: Classes.First, traveller: this.t1, id: 2, taken: false, isSelected: false};
    s3: Seat = {type: Classes.First, traveller: this.t1, id: 3, taken: false, isSelected: false};
    s4: Seat = {type: Classes.First, traveller: this.t1, id: 4, taken: false, isSelected: false};
    s5: Seat = {type: Classes.First, traveller: this.t1, id: 5, taken: true, isSelected: false};
    s6: Seat = {type: Classes.First, traveller: this.t1, id: 6, taken: false, isSelected: false};
    s7: Seat = {type: Classes.First, traveller: this.t1, id: 7, taken: false, isSelected: false};
    s8: Seat = {type: Classes.First, traveller: this.t1, id: 8, taken: false, isSelected: false};
    s9: Seat = {type: Classes.Business, traveller: this.t1, id: 9, taken: true, isSelected: false};
    s10: Seat = {type: Classes.Business, traveller: this.t1, id: 10, taken: false, isSelected: false};
    s11: Seat = {type: Classes.Business, traveller: this.t1, id: 11, taken: false, isSelected: false};
    s12: Seat = {type: Classes.Business, traveller: this.t1, id: 12, taken: true, isSelected: false};
    s13: Seat = {type: Classes.Economy, traveller: this.t1, id: 13, taken: true, isSelected: false};
    s14: Seat = {type: Classes.Economy, traveller: this.t1, id: 14, taken: true, isSelected: false};
    s15: Seat = {type: Classes.Economy, traveller: this.t1, id: 15, taken: true, isSelected: false};
    s16: Seat = {type: Classes.Economy, traveller: this.t1, id: 16, taken: true, isSelected: false};
    from: string;
    to: string;
    seats: Array<Seat>;
    airlines: Array<Airline>;
    stops: Array<Destination>;
    destinations: Array<Destination>;
    public flightsForDes = new Array<Flight>();
    time: Time = {hours: 2, minutes: 40};
    flights: Array<Flight>;
    flights2: Array<Flight>;
    flights3: Array<Flight>;
    flights4: Array<Flight>;
    povrtani: Flight;
    tickets: Array<Ticket>;
    l: Luggage;
    e: string;
    f: Flight;
    readonly BaseURI = 'http://localhost:57886/api';
    constructor( private http: HttpClient, private usrl: UserService) {
        this.l = new Luggage(20, '50x30x40', 2);
        this.e = 'Perfect your flight experience with full-flat seats, special food offers and other privileges.';
        this.destinations = [this.d1, this.d2, this.d3, this.d4];
        this.stops = [this.d1, this.d4];

        // tslint:disable-next-line: max-line-length
        this.f = new Flight(new Array<Ticket>(), this.povrtani, this.e, this.l, 0, new Array<number>(), 'Serbian air', Trip.One_way, this.seats, 1, this.d4, this.d1, new Date(2020, 4, 1, 0, 0, 0, 0), 300, '', 300, this.stops);
        // tslint:disable-next-line: max-line-length
        this.seats = [this.s1, this.s2, this.s3, this.s4, this.s5, this.s6, this.s7, this.s8, this.s9, this.s10, this.s11, this.s12, this.s13, this.s14, this.s15, this.s16];
        // tslint:disable-next-line: max-line-length
        this.flights = [{soldTickets: new Array<Ticket>(), extra: this.e, luggage: this.l, rate: 0, raters: new Array<number>(), idCompany: 'Serbian air', trip: Trip.Round_trip, seats: this.seats, id: 1, from: this.d1, to: this.d4, departureDate: new Date(2020, 4, 1, 0, 0, 0, 0), povratnilet: this.f, numOfPassengers: 300, prise: 300, duration: '', stops: this.stops},
                        // tslint:disable-next-line: max-line-length
                        {soldTickets: new Array<Ticket>(), extra: this.e, luggage: this.l, rate: 0, raters: new Array<number>(), idCompany: 'Serbian air', trip: Trip.One_way, seats: this.seats, id: 2, from: this.d4, to: this.d2, departureDate: new Date(2020, 3, 1, 0, 0, 0, 0), povratnilet: this.povrtani, numOfPassengers: 200, prise: 150, duration: '', stops: this.stops},
                        // tslint:disable-next-line: max-line-length
                        {soldTickets: new Array<Ticket>(), extra: this.e, luggage: this.l, rate: 0, raters: new Array<number>(), idCompany: 'Serbian air', trip: Trip.One_way, seats: this.seats, id: 3, from: this.d3, to: this.d1, departureDate: new Date(2020, 4, 1, 0, 0, 0, 0), povratnilet: this.povrtani, numOfPassengers: 250, prise: 100, duration: '', stops: this.stops},
                        // tslint:disable-next-line: max-line-length
                        {soldTickets: new Array<Ticket>(), extra: this.e, luggage: this.l, rate: 0, raters: new Array<number>(), idCompany: 'Serbian air', trip: Trip.One_way, seats: this.seats, id: 4, from: this.d2, to: this.d1, departureDate: new Date(2020, 4, 1, 0, 0, 0, 0), povratnilet: this.povrtani, numOfPassengers: 160, prise: 280, duration: '', stops: this.stops}];
    
        this.tickets = [{id: 1, flighth: this.flights[0], seat: this.s13, discount: 20},
                        // tslint:disable-next-line: max-line-length
                        {id: 2, flighth: this.flights[0], seat: this.s14, discount: 20},
                        // tslint:disable-next-line: max-line-length
                        {id: 3, flighth: this.flights[0], seat: this.s15, discount: 20},
                        // tslint:disable-next-line: max-line-length
                       {id: 4, flighth: this.flights[0], seat: this.s16, discount: 20}];
      
        // tslint:disable-next-line: max-line-length
        this.flights2 = [{soldTickets: new Array<Ticket>(), extra: this.e, luggage: this.l, rate: 0, raters: new Array<number>(), idCompany: 'Turkish airlines', trip: Trip.One_way, seats: this.seats, id: 5, from: this.d1, to: this.d4, departureDate: new Date(2020, 4, 1, 0, 0, 0, 0), povratnilet: this.povrtani, numOfPassengers: 300, prise: 300, duration: '', stops: this.stops},
                        // tslint:disable-next-line: max-line-length
                        {soldTickets: new Array<Ticket>(), extra: this.e, luggage: this.l, rate: 0, raters: new Array<number>(), idCompany: 'Turkish airlines', trip: Trip.One_way, seats: this.seats, id: 6, from: this.d4, to: this.d2, departureDate: new Date(2020, 4, 1, 0, 0, 0, 0), povratnilet: this.povrtani, numOfPassengers: 200, prise: 150, duration: '', stops: this.stops},
                        // tslint:disable-next-line: max-line-length
                        {soldTickets: new Array<Ticket>(), extra: this.e, luggage: this.l, rate: 0, raters: new Array<number>(), idCompany: 'Turkish airlines', trip: Trip.One_way, seats: this.seats, id: 7, from: this.d3, to: this.d1, departureDate: new Date(2020, 4, 1, 0, 0, 0, 0), povratnilet: this.povrtani, numOfPassengers: 250, prise: 100, duration: '', stops: this.stops},
                        // tslint:disable-next-line: max-line-length
                       {soldTickets: new Array<Ticket>(), extra: this.e, luggage: this.l, rate: 0, raters: new Array<number>(), idCompany: 'Turkish airlines', trip: Trip.One_way, seats: this.seats, id: 8, from: this.d2, to: this.d1, departureDate: new Date(2020, 4, 1, 0, 0, 0, 0), povratnilet: this.povrtani, numOfPassengers: 160, prise: 280, duration: '', stops: this.stops}];
        
        // tslint:disable-next-line: max-line-length
        this.flights3 = [{soldTickets: new Array<Ticket>(), extra: this.e, luggage: this.l, rate: 0, raters: new Array<number>(), idCompany: 'Montenegro airlines', trip: Trip.One_way, seats: this.seats, id: 9, from: this.d1, to: this.d4, departureDate: new Date(2020, 4, 1, 0, 0, 0, 0), povratnilet: this.povrtani, numOfPassengers: 300, prise: 300, duration: '', stops: this.stops},
                        // tslint:disable-next-line: max-line-length
                        {soldTickets: new Array<Ticket>(), extra: this.e, luggage: this.l, rate: 0, raters: new Array<number>(), idCompany: 'Montenegro airlines', trip: Trip.One_way, seats: this.seats, id: 10, from: this.d4, to: this.d2, departureDate: new Date(2020, 4, 1, 0, 0, 0, 0), povratnilet: this.povrtani, numOfPassengers: 200, prise: 150, duration: '', stops: this.stops},
                        // tslint:disable-next-line: max-line-length
                        {soldTickets: new Array<Ticket>(), extra: this.e, luggage: this.l, rate: 0, raters: new Array<number>(), idCompany: 'Montenegro airlines', trip: Trip.One_way, seats: this.seats, id: 11, from: this.d3, to: this.d1, departureDate: new Date(2020, 4, 1, 0, 0, 0, 0), povratnilet: this.povrtani, numOfPassengers: 250, prise: 100, duration: '', stops: this.stops},
                        // tslint:disable-next-line: max-line-length
                        {soldTickets: new Array<Ticket>(), extra: this.e, luggage: this.l, rate: 0, raters: new Array<number>(), idCompany: 'Montenegro airlines', trip: Trip.One_way, seats: this.seats, id: 12, from: this.d2, to: this.d1, departureDate: new Date(2020, 4, 1, 0, 0, 0, 0), povratnilet: this.povrtani, numOfPassengers: 160, prise: 280, duration: '', stops: this.stops}];

         // tslint:disable-next-line: max-line-length
        this.flights4 = [{soldTickets: new Array<Ticket>(), extra: this.e, luggage: this.l, rate: 0, raters: new Array<number>(), idCompany: 'Air France', trip: Trip.One_way, seats: this.seats, id: 13, from: this.d1, to: this.d4, departureDate: new Date(2020, 4, 1, 0, 0, 0, 0), povratnilet: this.povrtani, numOfPassengers: 300, prise: 300, duration: '', stops: this.stops},
                        // tslint:disable-next-line: max-line-length
                        {soldTickets: new Array<Ticket>(), extra: this.e, luggage: this.l, rate: 0, raters: new Array<number>(), idCompany: 'Air France', trip: Trip.One_way, seats: this.seats, id: 14, from: this.d4, to: this.d2, departureDate: new Date(2020, 4, 1, 0, 0, 0, 0), povratnilet: this.povrtani, numOfPassengers: 200, prise: 150, duration: '', stops: this.stops},
                        // tslint:disable-next-line: max-line-length
                        {soldTickets: new Array<Ticket>(), extra: this.e, luggage: this.l, rate: 0, raters: new Array<number>(), idCompany: 'Air France', trip: Trip.One_way, seats: this.seats, id: 15, from: this.d3, to: this.d1, departureDate: new Date(2020, 4, 1, 0, 0, 0, 0), povratnilet: this.povrtani, numOfPassengers: 250, prise: 100, duration: '', stops: this.stops},
                        // tslint:disable-next-line: max-line-length
                        {soldTickets: new Array<Ticket>(), extra: this.e, luggage: this.l, rate: 0, raters: new Array<number>(), idCompany: 'Air France', trip: Trip.One_way, seats: this.seats, id: 16, from: this.d2, to: this.d1, departureDate: new Date(2020, 4, 1, 0, 0, 0, 0), povratnilet: this.povrtani, numOfPassengers: 160, prise: 280, duration: '', stops: this.stops}];
        this.airlines = [
            // tslint:disable-next-line: max-line-length
            {Lat: 45.254410, Lon: 19.842550, fastTickets: this.tickets, raters: new Array<number>(), id: 1, admin: 1, name: 'Serbian air', address: 'Belgrade, Serbia', flights: this.flights, description: 'Svi direktni letovi kompanije Air Serbia Beograd na jednom mestu. Ukoliko želite direktno da letite za Pariz, Amasterdam, Njujork, Tivat, Rim, Milano i druge evropske gradove Air Serbia je pravi izbor za vas!', populardestinations: new Array<Destination>(this.d1), destinations: this.destinations, img: 'air-serbia.jpg', rating: 0},
            // tslint:disable-next-line: max-line-length
            {Lat: 45.254410, Lon: 19.842550, fastTickets: new Array<Ticket>(), raters: new Array<number>(), id: 2, admin: 12, name: 'Turkish airlines', address: 'Istanbul, Turkey', flights: this.flights2, description: 'Keep your airline tickets safe and secure, no hidden costs in just a few steps.', populardestinations: this.destinations, destinations:  this.destinations, img: 'turkish.jpg', rating: 0},
            // tslint:disable-next-line: max-line-length
            {Lat: 45.254410, Lon: 19.842550, fastTickets: new Array<Ticket>(), raters: new Array<number>(), id: 3, admin: 13, name: 'Montenegro airlines', address: 'Podgorica, Montenegro', flights: this.flights3, description: 'Keep your airline tickets safe and secure, no hidden costs in just a few steps.', populardestinations: this.destinations, destinations: this.destinations, img: 'montenegro.jpg',  rating: 0},
            // tslint:disable-next-line: max-line-length
            {Lat: 45.254410, Lon: 19.842550, fastTickets: new Array<Ticket>(), raters: new Array<number>(), id: 4, admin: 14, name: 'Air France', address: 'Paris, France', flights: this.flights4, description: 'Keep your airline tickets safe and secure, no hidden costs in just a few steps.', populardestinations: this.destinations, destinations: this.destinations, img: 'france.jpg',  rating: 0}
          ];
        var user = this.usrl.FindUserByEmail('ivana@gmail.com');
        user.oldFlights.push(new Ticket(1, this.flights[0], this.s1, 0));
    }
    addCompany(c: Airline)
    {
        this.airlines.push(c);
    }

    saveEditLocation(Id: number, oldLoc: string, newLoc: string, newD: string, newImg:string) {
        // tslint:disable-next-line: prefer-for-of
        for (let i = 0; i < this.airlines.length; i++){
            if(this.airlines[i].id == Id) {
                for (let j = 0; j < this.airlines[i].destinations.length; j++){
                    if(this.airlines[i].destinations[j].name == oldLoc) {
                        this.airlines[i].destinations[j].name = newLoc;
                        this.airlines[i].destinations[j].description = newD;
                        this.airlines[i].destinations[j].img = newImg;
                        return this.airlines[i];
                    }
                }
            }
        }
    }
    removeLocation(idA: number, idL: number)
    {
         // tslint:disable-next-line: prefer-for-of
         for (let i = 0; i < this.airlines.length; i++){
            if (this.airlines[i].id == idA) {
                // tslint:disable-next-line: prefer-for-of
                for (let j = 0; j < this.airlines[i].destinations.length; j++){
                    if (this.airlines[i].destinations[j].id == idL)
                    {
                        const index = this.airlines[i].destinations.indexOf(this.airlines[i].destinations[j]);
                        this.airlines[i].destinations.splice(index, 1);
                    }
                }
                return this.airlines[i];
            }
        }
    }
    AddLocation(id:number, locId:number, newLocName:string, des:string, newLocImg:string){
        for (let i = 0; i < this.airlines.length; i++){
            if (this.airlines[i].id == id) {
                this.airlines[i].destinations.push(new Destination(locId, newLocName, newLocImg, des));
                return this.airlines[i];
            }
        }
    }
    loadTickets(idA: number)
    {   
        // tslint:disable-next-line: prefer-for-of
        for (let i = 0; i < this.airlines.length; i++){
            // tslint:disable-next-line: triple-equals
            if (this.airlines[i].id == idA)
            {
                return this.airlines[i].fastTickets;
            }
        }
    }
    loadAirlineByName(id: string)
    {
         // tslint:disable-next-line: prefer-for-of
         for (let i = 0; i < this.airlines.length; i++){
            // tslint:disable-next-line: triple-equals
            if (this.airlines[i].name == id)
            {
                return this.airlines[i];
            }
        }
    }
    loadFlight(idF: number, idA: number)
    {
        // tslint:disable-next-line: prefer-for-of
        for (let i = 0; i < this.airlines.length; i++){
            // tslint:disable-next-line: triple-equals
            if (this.airlines[i].id == idA)
            {
                this.a = this.airlines[i];
            }
        }
        // tslint:disable-next-line: prefer-for-of
        for (let i = 0; i < this.a.flights.length; i++){
            // tslint:disable-next-line: triple-equals
            if (this.a.flights[i].id == idF)
            {
                return this.a.flights[i];
            }
        }
    }
    
    flightsForDestination(idA: number, desName: string)
    {
        this.flightsForDes = new Array<Flight>();
        // tslint:disable-next-line: prefer-for-of
        for (let i = 0; i < this.airlines.length; i++){
            // tslint:disable-next-line: triple-equals
            if (this.airlines[i].id == idA)
            {
                this.a = this.airlines[i];
            }
        }
        // tslint:disable-next-line: prefer-for-of
        for (let i = 0; i < this.a.flights.length; i++){
            // tslint:disable-next-line: triple-equals
            if (this.a.flights[i].to.name == desName)
            {
                this.flightsForDes.push(this.a.flights[i]);
            }
        }
        return this.flightsForDes;
    }
    loadAllFlights()
    {
        this.flightsForDes = new Array<Flight>();
        // tslint:disable-next-line: prefer-for-of
        for (let i = 0; i < this.airlines.length; i++){
            // tslint:disable-next-line: prefer-for-of
            for (let j = 0; j < this.airlines[i].flights.length; j++){
                // tslint:disable-next-line: triple-equals
                this.flightsForDes.push(this.airlines[i].flights[j]);
            }
        }
        return this.flightsForDes;
    }
    findSeat(idSeat: number, idA: number, idF: number)
    {
         // tslint:disable-next-line: prefer-for-of
        for (let i = 0; i < this.airlines.length; i++){
            // tslint:disable-next-line: triple-equals
            if (this.airlines[i].id == idA)
            {
                this.a = this.airlines[i];
            }
        }
        // tslint:disable-next-line: prefer-for-of
        for (let i = 0; i < this.a.flights.length; i++){
            // tslint:disable-next-line: triple-equals
            if (this.a.flights[i].id == idF)
            {
                // tslint:disable-next-line: prefer-for-of
                for (let h = 0; h < this.a.flights[i].seats.length; h++){
                    // tslint:disable-next-line: triple-equals
                    if (this.a.flights[i].seats[h].id == idSeat)
                    {
                        return this.a.flights[i].seats[h];
                    }
                }
            }
        }
    }

    getCompany(id:number) {
        for (let i = 0; i < this.airlines.length; i++){
            if (this.airlines[i].admin == id)
            {
                return this.airlines[i];
            }
        }
    }


   

    containsDestination(loc: string, id: number)
    {
        
        // tslint:disable-next-line: prefer-for-of
        for (let i = 0; i < this.airlines.length; i++){
            if (this.airlines[i].id == id)
            {
                // tslint:disable-next-line: prefer-for-of
                for (let j = 0; j < this.airlines[i].destinations.length; j++)
                {
                    if (this.airlines[i].destinations[j].name == loc)
                    {
                        return true;
                    }
                }
            }
        }       
        return false;   
    }
    findDestination(name: string, id: number)
    {
         // tslint:disable-next-line: prefer-for-of
         for (let i = 0; i < this.airlines.length; i++){
            if (this.airlines[i].id == id)
            {
                // tslint:disable-next-line: prefer-for-of
                for (let j = 0; j < this.airlines[i].destinations.length; j++)
                {
                    if (this.airlines[i].destinations[j].name == name)
                    {
                        return this.airlines[i].destinations[j];
                    }
                }
            }
        }         
    }

    getAllAirlines() {
        return this.airlines;
    }

    addNewCompany(name:string, adminId: number, address:Address) {
        var company = new Airline(address.longitude, 
            address.latitude, new Array<Ticket>(), adminId, new Array<number>(), this.airlines.length + 50,
            name, address.fullAddress, '', new Array<Destination>(),  new Array<Destination>(),
            new Array<Flight>(), '', 0);
        this.airlines.push(company);
        return company;
    }
    getAllCompanies() {
        return this.http.get(this.BaseURI + '/Airline/GetAllActivatedCompanies');
    }
    getAllFlights() {
        var flighs = this.http.get(this.BaseURI + '/Airline/GetAllFlights');
        return flighs;
    }
    searchCompanies(searchModel) {

        return this.http.post(this.BaseURI + '/Airline/SearchCompanies', searchModel);
    }
    searchFlights(searchModel) {

        return this.http.post(this.BaseURI + '/Airline/SearchFlights', searchModel);
    }
    loadFlightById(idModel) {

        return this.http.post(this.BaseURI + '/Airline/LoadFlight', idModel);
    }
    loadAirline(idModel)
    {                                                   
        return this.http.post(this.BaseURI + '/Airline/LoadCompany', idModel);
    }
    addTraveller(model)
    {                                                   
        return this.http.post(this.BaseURI + '/AppUser/AddTraveller', model);
    }
    UpdateCompany(model) {
        return this.http.post(this.BaseURI + '/Airline/UpdateCompany', model);
    }
    SaveEditLocation(model) {
        return this.http.post(this.BaseURI + '/Airline/SaveEditLocation', model);
    }
    RemoveLocation(model) {
        return this.http.post(this.BaseURI + '/Airline/RemoveLocation', model);
    }
    RemoveFlight(model) {
        return this.http.post(this.BaseURI + '/Airline/RemoveFlight', model);
    }
    EditFlight(model) {
        return this.http.post(this.BaseURI + '/Airline/EditFlight', model);
    }
    AddToPopular(model)
    {
        return this.http.post(this.BaseURI + '/Airline/AddToPopular', model);

    }
    AddNewDestination(model)
    {
        return this.http.post(this.BaseURI + '/Airline/AddNewDestination', model);

    }
    AddNewFlight(model)
    {
        return this.http.post(this.BaseURI + '/Airline/AddNewFlight', model);
    }
    NewFastTicket(model)
    {
        return this.http.post(this.BaseURI + '/Airline/NewFastTicket', model);
    }
  
}
