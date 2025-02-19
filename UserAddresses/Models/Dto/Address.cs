﻿namespace UserAddresses.Models.Dto;

public class Address
{
    public int Id { get; set; }
    
    public string Street { get; set; }
    
    public string City { get; set; }
    
    public string Postcode { get; set; }

    public string Country { get; set; }
}