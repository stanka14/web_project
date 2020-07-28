import { Injectable } from '@angular/core';
import { Time } from '@angular/common';
import { Car } from 'src/app/entities/car/car';
import { UserService } from './userService';
import { CarReservation } from 'src/app/entities/carReservation/carReservation'
import { Address } from '../entities/address/address';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({
    providedIn: 'root',
   })

export class CarRentalService {
    readonly BaseURI = 'http://localhost:57886/api';

    

    constructor(private usrl: UserService, private http: HttpClient) {

        

        

    }

    // loadCompany(id: number)
    // {
    //     for (let i = 0; i < this.carCompanies.length; i++){
    //         if (this.carCompanies[i].id == id)
    //         {
    //             return this.carCompanies[i];
    //         }
    //     }
    // }

    // loadCar(id: number) 
    // {
    //     for (let i = 0; i < this.cars.length; i++){
    //         if (this.cars[i].id == id)
    //         {
    //             return this.cars[i];
    //         }
    //     }
    //     for (let i = 0; i < this.cars2.length; i++){
    //         if (this.cars2[i].id == id)
    //         {
    //             return this.cars2[i];
    //         }
    //     }
    //     for (let i = 0; i < this.cars3.length; i++){
    //         if (this.cars3[i].id == id)
    //         {
    //             return this.cars3[i];
    //         }
    //     }
    //     for (let i = 0; i < this.cars4.length; i++){
    //         if (this.cars4[i].id == id)
    //         {
    //             return this.cars4[i];
    //         }
    //     }
    //     for (let i = 0; i < this.cars5.length; i++){
    //         if (this.cars5[i].id == id)
    //         {
    //             return this.cars5[i];
    //         }
    //     }
    //     for (let i = 0; i < this.cars6.length; i++){
    //         if (this.cars6[i].id == id)
    //         {
    //             return this.cars6[i];
    //         }
    //     }
    //     for (let i = 0; i < this.cars7.length; i++){
    //         if (this.cars7[i].id == id)
    //         {
    //             return this.cars7[i];
    //         }
    //     }
    //     for (let i = 0; i < this.cars8.length; i++){
    //         if (this.cars8[i].id == id)
    //         {
    //             return this.cars8[i];
    //         }
    //     }
    // }

    // loadAllCars() {
    //     this.allCars = new Array<Car>();
    //     for (let i = 0; i < this.cars.length; i++){
    //         this.allCars.push(this.cars[i]);
    //     }
    //     for (let i = 0; i < this.cars2.length; i++){
    //         this.allCars.push(this.cars2[i]);
    //     }
    //     for (let i = 0; i < this.cars3.length; i++){
    //         this.allCars.push(this.cars3[i]);
    //     }
    //     for (let i = 0; i < this.cars4.length; i++){
    //         this.allCars.push(this.cars4[i]);
    //     }
    //     for (let i = 0; i < this.cars5.length; i++){
    //         this.allCars.push(this.cars5[i]);
    //     }
    //     for (let i = 0; i < this.cars6.length; i++){
    //         this.allCars.push(this.cars6[i]);
    //     }
    //     for (let i = 0; i < this.cars7.length; i++){
    //         this.allCars.push(this.cars7[i]);
    //     }
    //     for (let i = 0; i < this.cars8.length; i++){
    //         this.allCars.push(this.cars8[i]);
    //     }
        
    //     return this.allCars;
    // }

    

    // UpdateCompany(id:number, name:string, desc:string, img:string) {
    //     for (let i = 0; i < this.carCompanies.length; i++){
    //         if(this.carCompanies[i].id == id) {
    //             this.carCompanies[i].name = name;
    //             this.carCompanies[i].description = desc;
    //             this.carCompanies[i].img = img;
    //             if(!this.carCompanies[i].activated) {
    //                 this.carCompanies[i].activated = true;
    //             }
    //             return this.carCompanies[i];
    //         }
    //     }
    // }

    // addLocation(Id:number, loc:string) {
    //     for (let i = 0; i < this.carCompanies.length; i++){
    //         if(this.carCompanies[i].id == Id) {
    //             this.carCompanies[i].locations.push(new Address(loc, 45, 20));
    //             return this.carCompanies[i];
    //         }
    //     }
    // }

    // saveEditLocation(Id:number, oldLoc:string, newLoc:string) {
    //     for (let i = 0; i < this.carCompanies.length; i++){
    //         if(this.carCompanies[i].id == Id) {
    //             for (let j = 0; j < this.carCompanies[i].locations.length; j++){
    //                 if(this.carCompanies[i].locations[j].fullAddress == oldLoc) {
    //                     this.carCompanies[i].locations[j].fullAddress = newLoc;
    //                     return this.carCompanies[i];
    //                 }
    //             }
    //         }
    //     }
    // }

    // updateCar(compId:number, id:number, brand:string, model:string, year:number, type:string, passen:number, loc:string, price:number) {
    //     for (let i = 0; i < this.carCompanies.length; i++){
    //         if(this.carCompanies[i].id == compId) {
    //             for (let j = 0; j < this.carCompanies[i].cars.length; j++){
    //                 if(this.carCompanies[i].cars[j].id == id) {
    //                     this.carCompanies[i].cars[j].brand = brand;
    //                     this.carCompanies[i].cars[j].model = model;
    //                     this.carCompanies[i].cars[j].year = year;
    //                     this.carCompanies[i].cars[j].type = type;
    //                     this.carCompanies[i].cars[j].passengers = passen;
    //                     this.carCompanies[i].cars[j].pricePerDay = price;
    //                     this.carCompanies[i].cars[j].location = loc;
    //                     return this.carCompanies[i];
    //                 }
    //             }
    //         }
    //     }
    // }

    // addCar(id:number, brand:string, model:string, type:string, year:number, price:number, passengers:number, location:string) {
    //     for (let i = 0; i < this.carCompanies.length; i++){
    //         if(this.carCompanies[i].id == id) {
    //             this.carCompanies[i].cars.push(new Car((50+this.carCompanies[i].cars.length), this.carCompanies[i].id, brand, model, year, price, type, passengers, location, new Array<string>(), '4star.png', new Array<number>(), 0));
    //             return this.carCompanies[i];
    //         }
    //     }
    // }

    // checkIfAvailable(id: number, idCar: number, dates: Array<string>) {
    //     for (let i = 0; i < this.carCompanies.length; i++){
    //         if(this.carCompanies[i].id == id) {
    //             for(let j = 0; j<this.carCompanies[i].cars.length; j++) {
    //                 if(this.carCompanies[i].cars[j].id == idCar) {
    //                     for(let k = 0; k<dates.length; k++) {
    //                         if(this.carCompanies[i].cars[j].rented.includes(dates[k])){
    //                             return false;
    //                         }
    //                     }
    //                     return true;
    //                 }
    //             }
    //         }
    //     }
    // }

    // checkBranch(id: number, retLoc: string) {
    //     for (let i = 0; i < this.carCompanies.length; i++){
    //         if(this.carCompanies[i].id == id) {
    //             for(let j = 0; j<this.carCompanies[i].locations.length; j++) {
    //                 if (this.carCompanies[i].locations[j].fullAddress.toLowerCase() == retLoc.toLowerCase()) {
    //                     return true;
    //                 }
    //             }
    //             return false;
    //         }
    //     }
    // }

    // findCompanyOfCar(id: number) {
    //     for(let i = 0; i<this.carCompanies.length; i++) {
    //         for(let j = 0; j<this.carCompanies[i].cars.length; j++) {
    //             if(this.carCompanies[i].cars[j].id == id) {
    //                 return this.carCompanies[i];
    //             }
    //         }
    //     }
    // }

    // rateCompany(companyId: number, star:number) {
    //     for(let i = 0; i<this.carCompanies.length; i++) {
    //         if(this.carCompanies[i].id == companyId) {
    //             this.carCompanies[i].stars.push(star);
    //             var sum = 0;
    //             for(let j = 0; j<this.carCompanies[i].stars.length; j++) {
    //                 sum = sum + this.carCompanies[i].stars[j];
    //             }
    //             this.carCompanies[i].rating = Math.round(sum / this.carCompanies[i].stars.length);
    //         }
    //     }
    // }

    // // rateCar(companyId: number, car: Car, star:number) {
    // //     for(let i = 0; i<this.carCompanies.length; i++) {
    // //         if(this.carCompanies[i].id == companyId) {
    // //             for(let k = 0; k<this.carCompanies[i].cars.length; k++) {
    // //                 if(this.carCompanies[i].cars[k].id == car.id) {
    // //                     this.carCompanies[i].cars[k].stars.push(star);
    // //                     var sum = 0;
    // //                     for(let j = 0; j<this.carCompanies[i].cars[k].stars.length; j++) {
    // //                         sum = sum + this.carCompanies[i].cars[k].stars[j];
    // //                     }
    // //                     this.carCompanies[i].cars[k].avrageStar = Math.round(sum / this.carCompanies[i].stars.length);
    // //                 }
    // //             }
                
    // //         }
    // //     }
    // // }

    // getDiscountedCars(location: string, from: Date, to: Date) {
    //     var MS_PER_DAY = 1000 * 60 * 60 * 24;
    //     var start = from.getTime();
    //     var end = to.getTime();
    //     var days = Math.ceil((end - start) / MS_PER_DAY);

    //     var datesArray = Array.from(new Array(days + 1), (v, i) => new Date(start + (i * MS_PER_DAY)));
    //     var dates = new Array<string>();   //
    //     for(let i = 0; i<datesArray.length; i++) {
    //         dates.push(datesArray[i].toDateString());
    //     }
    //     var cars = new Array<Car>();
    //     for(let i = 0; i<this.carCompanies.length; i++) {
    //         for(let j = 0; j<this.carCompanies[i].discountedCars.length; j++) {
    //             if(this.carCompanies[i].discountedCars[j][0].location == location) {
    //                 var ok = true;
    //                 for(let k = 0; k<dates.length; k++) {
    //                     if (!this.carCompanies[i].discountedCars[j][1].includes(dates[k])) {
    //                         ok = false;
    //                     }
    //                 }
    //                 if(ok){
    //                     cars.push(this.carCompanies[i].discountedCars[j][0]);
    //                 }
    //             }
    //         }
    //     }
    //     return cars;
    // }

    

    // addCompany(name:string, adminId: number, address:Address) {
    //     var company = new RentAcar(false, [], this.carCompanies.length + 20,adminId, name, address, "", 0, [], "companyPlaceholder.jpg", [], [], []);
    //     this.carCompanies.push(company);
    //     return company;
    // }

    // getAllActivatedCompanies() {
    //     var companies = [];
    //     for(let i = 0; i<this.carCompanies.length; i++) {
    //         if(this.carCompanies[i].activated) {
    //             companies.push(this.carCompanies[i]);
    //         }
    //     }
    //     return companies;
    // }

    getAllCompanies() {
        return this.http.get(this.BaseURI + '/RentACar/GetAllActivatedCompanies');
    }

    searchCompanies(searchModel) {
        return this.http.post(this.BaseURI + '/RentACar/SearchCompanies', searchModel);
    }

    // getCompany(id: number) {
    //     for (let i = 0; i < this.carCompanies.length; i++){
    //         if(this.carCompanies[i].admin == id) {
    //             return this.carCompanies[i];
    //         }
    //     }
    // }

    getCompanyProfile(idModel) {
        return this.http.post(this.BaseURI + '/RentACar/GetCompanyProfile', idModel);
    }

    getCarsSearch(searchModel) {
        return this.http.post(this.BaseURI + '/RentACar/SearchCars', searchModel);
    }

    getCar(idModelCarComp) {
        var idModel = {
            idComp: parseInt(idModelCarComp.idComp),
            idCar: parseInt(idModelCarComp.idCar)
        }
        return this.http.post(this.BaseURI + '/RentACar/GetCarInfo', idModel);
    }

    getCarsSearchHome(searchModel) {
        return this.http.post(this.BaseURI + '/RentACar/SearchCarsHome', searchModel);
    }
    getCompanyAmenities(idModel) {
        return this.http.get(this.BaseURI + '/RentACar/GetCompanyAmenities', {params: {id: idModel}});
    }

    rentCar(res) {
        var reservation = {
            company: parseInt(res.company),
            car: parseInt(res.car),
            from: res.from,
            to: res.to,
            pickUpAddr: res.pickUpAddr,
            dropOffAddr: res.dropOffAddr,
            fromTime: res.fromTime,
            toTime: res.toTime,
            extras: res.extras
        }
        return this.http.post(this.BaseURI + '/AppUser/RentCar', reservation);
    }

    GiveUpCarRes(companyId: number, resId: number) {
        var model = {
            compId: companyId,
            resId: resId
        }
        return this.http.post(this.BaseURI + '/AppUser/GiveUpCarReservation', model);
    }

    rateCarCompany(companyId: number, star:number, resId: number) {
        var model = {
            compId: companyId,
            star: star,
            resId: resId
        }
        return this.http.post(this.BaseURI + '/AppUser/RateCarCompany', model);
    }

    rateCar(compId:number, star:number, resId: number, carId: number) {
        var model = {
            compId: compId,
            star: star,
            resId: resId,
            carId: carId
        }
        return this.http.post(this.BaseURI + '/AppUser/RateCar', model);
    }

    getCompanyInfo(companyId) {
        return this.http.get(this.BaseURI + '/RentACar/GetCompanyInfoAdmin', {params: {id: companyId}});
    }
    updateCarCompany(companyId:number, name:string, desc:string, img:string){
        var model = {
            compId: companyId,
            name: name,
            description: desc,
            logo: img
        }
        return this.http.post(this.BaseURI + '/AppUser/UpdateCompanyInfo', model);
    }
    addCarCompanyLocation(companyId:number, clickedAddress:string, latitude:number, longitude:number) {
        var model = {
            compId: companyId,
            address: clickedAddress,
            latitude: latitude,
            longitude: longitude
        }
        return this.http.post(this.BaseURI + '/AppUser/AddCarCompanyLocation', model);
    }
    editCarCompanyLocation(companyId:number, clickedAddress:string, latitude:number, longitude:number, id:number) {
        var model = {
            compId: companyId,
            address: clickedAddress,
            latitude: latitude,
            longitude: longitude,
            locId: id
        }
        return this.http.post(this.BaseURI + '/AppUser/EditCarCompanyLocation', model);
    }
    removeCarCompanyLocation(id:number, companyId: number, newLoc: string) {
        var model = {
            id: id,
            id2: companyId,
            newAddr: newLoc
        }
        return this.http.post(this.BaseURI + '/AppUser/RemoveCarCompanyLocation', model);
    }
    UpdateCar(model){
        return this.http.post(this.BaseURI + '/AppUser/UpdateCar', model);
    }
    addNewCar(model) {
        return this.http.post(this.BaseURI + '/AppUser/AddNewCar', model);
    }
    updateAmenity(model) {
        return this.http.post(this.BaseURI + '/AppUser/UpdateAmenity', model);
    }
    AddAmenity(model) {
        return this.http.post(this.BaseURI + '/AppUser/AddAmenity', model);
    }
    SaveNewDiscountRange(model){
        return this.http.post(this.BaseURI + '/AppUser/SaveNewDiscountRange', model);
    }
    RemoveDiscountRange(id: number) {
        var model = {
            id: id,
        }
        return this.http.post(this.BaseURI + '/AppUser/RemoveDiscountRange', model);
    }
    RemoveCar(id: number) {
        var model = {
            id: id,
        }
        return this.http.post(this.BaseURI + '/AppUser/RemoveCar', model);
    }
    removeAmenity(model) {
        return this.http.post(this.BaseURI + '/AppUser/RemoveAmenity', model);
    }
    getDiscountedCars(id:number){
        return this.http.get(this.BaseURI + '/RentACar/GetDiscountedCarsForCompany', {params: {id: id.toString()}});
    }
}

