﻿using Business.Abstract;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validaton;
using Core.Utilities.Business;
using Core.Utilities.Result;
using DataAccess.Abstract;
using DataAccess.Concrete.Entityframework;
using Entities.Concrete;
using Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class RentalManager : IRentalService
    {
        IRentalDal _rentalDal;

        public RentalManager(IRentalDal rentalDal)
        {
            _rentalDal = rentalDal;
        }

        

        public IDataResult<List<Rental>> GetAll()
        {
            return new SuccessDataResult<List<Rental>>(_rentalDal.GetAll(), Messages.RentalListed);
        }

        

        public IDataResult<Rental> GetById(int id)
        {
            return new SuccessDataResult<Rental>(_rentalDal.Get(r => r.Id == id), Messages.RentalByListed);
        }


        //-----------------------------RentalDetailDto---------------------------------------
        public IDataResult<List<RentalDetailDto>> GetRentalDetails()
        {
            return new SuccessDataResult<List<RentalDetailDto>>(_rentalDal.GetRentalDetails(), Messages.RentalListed);
        }


        public IDataResult<RentalDetailDto> GetByCarIdRentalDetails(int carId)
        {
            return new SuccessDataResult<RentalDetailDto>(_rentalDal.GetRentalDetails(c => c.CarId == carId).FirstOrDefault());
        }

        
        public IDataResult<bool> CheckIfCanACarBeRented(int carId)//Araç kiralanabilir mi?
        {
            var result = BusinessRules.Run(CheckIfCarAvailableNow(carId));

            if (result != null)
            {
                return new ErrorDataResult<bool>(false,result.Message); ;
            }

            return new SuccessDataResult<bool>(true);
        }

        public IDataResult<bool> CheckIfAnyRentalBetweenSelectedDates(int carId, DateTime rentDate, DateTime returnDate)
        {
            if (rentDate>DateTime.Now)
            {
                return CheckIfCarAvailableBetweenSelectedDatesForReservations(carId, rentDate, returnDate);
            }

            return CheckIfCarAvailableBetweenSelectedDatesForCurrentRentals(carId, rentDate, returnDate);
        }






        //-----------------------Add,Update,Delete ----------------------------

        [ValidationAspect(typeof(RentalValidator))]
        public IResult Add(Rental rental)
        {
            IResult rulesResult;

            if (rental.RentDate > DateTime.Now) //Rezervasyon
            {
                rulesResult = BusinessRules.Run(
                    CheckIfCarAvailableBetweenSelectedDatesForReservations(rental.CarId, rental.RentDate, rental.ReturnDate),
                    CheckDeliveryStatus(rental));

                
            }
            else //Anlık kiralama
            {
                rulesResult = BusinessRules.Run(
                    CheckIfCarAvailableNow(rental.CarId),
                    CheckDeliveryStatus(rental));
            }
            if (rulesResult != null)
            {
                return rulesResult;
            }
            _rentalDal.Add(rental);
            return new SuccessResult(Messages.RentalAdded);
        }
 

        public IResult Delete(Rental rental)
        {
            _rentalDal.Delete(rental);
            return new SuccessResult(Messages.RentalDeleted);
        }


        [ValidationAspect(typeof(RentalValidator))]
        public IResult Update(Rental rental)
        {
            _rentalDal.Update(rental);
            return new SuccessResult(Messages.RentalUpdated);
        }

        public IResult CarDeliver(int rentalId)// Araç geir teslim fonk ReturnDate =---> DateTime.now
        {
            _rentalDal.CarDeliver(rentalId);
            return new SuccessResult(Messages.CarIsDelivered);
        }




        //---------------------------------------------Business Rules ------------------
        private IResult CheckIfCarAvailableNow(int carId)
        {
            // kiralanmak istenen araçın id si kiralama başlama tarihini şimdiki zamandan küçük,teslim tarihi şimdiki zamandan büyük
            // ve araç teslim edilmemiş ise bu araç şuanda kiralık
            
            var isCarRented = _rentalDal.GetAll(r => r.CarId == carId && r.RentDate <= DateTime.Now && r.ReturnDate >= DateTime.Now && r.DeliveryStatus == false).Any();

            if (isCarRented) // araba kiralanmış ise kullanıcıyı bildir
            {
                return new ErrorResult(Messages.CarNotEmty);
            }
            return new SuccessResult();
        }

        private IDataResult<bool> CheckIfCarAvailableBetweenSelectedDatesForReservations(int carId, DateTime rentDate, DateTime returnDate)
        {
            return BaseCheckIfCarAvailableBetweenSelectedDates(carId, rentDate, returnDate,null);
        }

        private IDataResult<bool> CheckIfCarAvailableBetweenSelectedDatesForCurrentRentals(int carId, DateTime rentDate, DateTime returnDate)
        {
            return BaseCheckIfCarAvailableBetweenSelectedDates(carId, rentDate, returnDate, false);
        }
        private IDataResult<bool> BaseCheckIfCarAvailableBetweenSelectedDates(int carId,DateTime rentDate,DateTime returnDate,bool? deliveryStatus)
        {
            var allRentals=_rentalDal.GetAll(r=>r.CarId==carId && r.DeliveryStatus==deliveryStatus);

            foreach (var reservation in allRentals)
            {
                if ((rentDate >= reservation.RentDate && rentDate <= reservation.ReturnDate) ||
                    (returnDate >= reservation.RentDate && returnDate <= reservation.ReturnDate) ||
                    (reservation.RentDate >= rentDate && reservation.RentDate <= returnDate) ||
                    (reservation.ReturnDate >= rentDate && reservation.ReturnDate <= returnDate))
                {
                    

                return new ErrorDataResult<bool>(false, Messages.ReservationBetweenSelectedDatesExist);

                }
                
            }
            return new SuccessDataResult<bool>(true, Messages.CarCanBeRentedBetweenSelectedDates);
        }

        private IResult CheckDeliveryStatus(Rental rental)
        {
            //Kiralama geçmişte isee
            if (rental.RentDate <=DateTime.Now && rental.ReturnDate<=DateTime.Now)
            {
                if (rental.DeliveryStatus==null)
                {
                    return new ErrorResult(Messages.DeliveryStatusMustNotBeNull);
                }
                return new SuccessResult();
            }
            
            //Kiralama şuanda aktif  ises
            else if (rental.RentDate <= DateTime.Now && rental.ReturnDate >= DateTime.Now)
            {
                if (rental.DeliveryStatus != false)
                {
                    return new ErrorResult(Messages.DeliveryStatusMustFalse);
                }
                return new SuccessResult();
            }

            //Kiralama ileri ki tarihte ise ,rezervasyon ise
            else 
            {
                if (rental.DeliveryStatus != null)
                {
                    return new ErrorResult(Messages.DeliveryStatusMustNull);
                }
                return new SuccessResult();
            }

        }

       
    }
}
