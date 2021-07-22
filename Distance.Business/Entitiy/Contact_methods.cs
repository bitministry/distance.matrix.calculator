using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Distance.Business.Interfaces;

namespace Distance.Business.Entitiy
{
    public partial class Contact 
    {

        public void Delete(IGenericRepository<Contact> contactRepository,
            IGenericRepository<Address> addressRepository, 
            IGenericRepository<DistanceComparison> discoRepository, 
            IGenericRepository<ComparisonReport> reportRepository )
        {
            var addresses = Addresses.Select(x => x.AddressId).ToArray();
            foreach (var addressId in addresses)
                addressRepository.Delete(addressId);

            var compresAffected = reportRepository.Find(x => x.DistanceComparison.Customer.ContactId == this.ContactId ||
                x.DistanceComparison.CompetitorsIncluded.Any(y => y.ContactId == this.ContactId)).Select( x=> x.ComparisonReportId ).ToArray();

            foreach (var reportId in compresAffected)
                reportRepository.Delete(reportId);

            var discosAffected = discoRepository.Find(x => x.Customer.ContactId == this.ContactId ||
                x.CompetitorsIncluded.Any(y => y.ContactId == this.ContactId)).Select(x => x.DistanceComparisonId).ToArray(); ;
            
            foreach (var discoId in discosAffected)
                discoRepository.Delete(discoId);

            contactRepository.Delete(this); 

        }

        // validate, trim keys and remove duplicates 

        IList<Address> ProccessPostcodes( 
            IEnumerable<string> newPostCodes, 
            string validatorCountryIso3Code )
        {
            newPostCodes = newPostCodes.Where(x => !String.IsNullOrWhiteSpace(x)).OrderBy(x => x);

            // remove duplicates if not Compeditor
            if ( ContactType == Entitiy.ContactType.Competitor )
                newPostCodes = newPostCodes.Distinct();

            if (String.IsNullOrWhiteSpace(validatorCountryIso3Code) || validatorCountryIso3Code == "Any" || 
                validatorCountryIso3Code.Length != 3 )
            {
                return newPostCodes                    
                    .Select(x => new Address { PostCode = x })
                    .ToList();
            }

            var validatedAddresses = new List<Address>();

            foreach (var postcode in newPostCodes)
            {
                try
                {
                    var address = new Address(validatorCountryIso3Code, postcode);
                    if (ContactType == Entitiy.ContactType.Competitor && validatedAddresses.Contains(address))
                        continue; 
                    validatedAddresses.Add(address);
                }
                catch (InvalidDataException ex)
                {
                    if (InvalidPostCodes == null)
                        InvalidPostCodes = new List<string>();
                    InvalidPostCodes.Add(postcode);
                }
            }
            return validatedAddresses; 
        }

        public void UpdateAddressesByPostcodes(string[] newPostCodes, 
            string validatorCountryIso3Code, 
            IGenericRepository<Contact> contactRepository ,
            IGenericRepository<Address> addressRepository )
        {
            var listAddressesNew = ProccessPostcodes(newPostCodes, validatorCountryIso3Code);
            var listPostCodesNew = listAddressesNew.Select(x => x.PostCode).ToList(); 

            var listPostCodesExisting = Addresses
                .Select(x => x.PostCode)
                .OrderBy(x => x ).ToList(); 

            // rerturn when no changes ..
            if (listPostCodesNew.SequenceEqual(listPostCodesExisting)) return;

//            var postCodesToDelete = listPostCodesExisting.Except(listPostCodesNew);
            var postCodesToDelete = StringsInSecondUnexistinfInFirst(listPostCodesNew, listPostCodesExisting);

            foreach (var postCode in postCodesToDelete)
            {
                var address = Addresses.FirstOrDefault(x => x.PostCode == postCode);
                Addresses.Remove(address);
                addressRepository.DeleteShallow(address);
            }

            var postCodesToInsert = StringsInSecondUnexistinfInFirst(listPostCodesExisting, listPostCodesNew );
            var addressesToInsert = postCodesToInsert.Select(x => new Address() { PostCode = x, Contact = this });
            foreach (var address in addressesToInsert)
            {
                Addresses.Add(address);
                addressRepository.InsertShallow(address);
            }

            AddressesModified = DateTime.Now; 
            contactRepository.Save(this);
            addressRepository.PersistUnsavedChanges();

        }

        #region currently unused Methods for single address adding and removing .. 

        public void AddAddress(Address newAdr,
            IGenericRepository<Contact> contactRepository ,
            IGenericRepository<Address> addressRepository )
        {
            if (Addresses.Any(a => a.PostCode == newAdr.PostCode))
                throw new InvalidDataException("Duplicate address");

            newAdr.Contact = this;
            Addresses.Add(newAdr);
            AddressesModified = DateTime.Now;

            if (contactRepository == null || addressRepository == null)
                return;

            contactRepository.Save(this);
            addressRepository.Save(newAdr);
        }

        public void RemoveAddress(Address oldAdr,
            IGenericRepository<Contact> contactRepository = null,
            IGenericRepository<Address> addressRepository = null)
        {
            if (Addresses.Contains(oldAdr))
                throw new InvalidDataException("No such address");

            Addresses.Remove(oldAdr);
            AddressesModified = DateTime.Now;

            if (contactRepository == null || addressRepository == null)
                return;

            contactRepository.Save(this);
            addressRepository.Delete(oldAdr);
        }



        #endregion

        IList<string> StringsInSecondUnexistinfInFirst(IList<string> first, IList<string> second)
        {
            var ret = second.ToList(); 
            foreach (var xs in first)
                ret.Remove(xs);
            return ret; 
        }

    }
}
