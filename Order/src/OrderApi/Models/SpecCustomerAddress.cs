﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace OrderApi.Models
{
    public partial class SpecCustomerAddress
    {
        public int SpecCustomerAddressId { get; set; }
        public int? CustomerId { get; set; }
        public int? AddressId { get; set; }

        public virtual Address Address { get; set; }
        public virtual Customer Customer { get; set; }
    }
}