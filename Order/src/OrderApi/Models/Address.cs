﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace OrderApi.Models
{
    public partial class Address
    {
        public Address() {
            Order = new HashSet<Order>();
            SpecCustomerAddress = new HashSet<SpecCustomerAddress>();
        }

        public int AddressId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string City { get; set; }

        public virtual ICollection<Order> Order { get; set; }
        public virtual ICollection<SpecCustomerAddress> SpecCustomerAddress { get; set; }
    }
}