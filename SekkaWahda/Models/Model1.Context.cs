﻿

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace SekkaWahda.Models
{

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;


public partial class SECURITY_DBEntities : DbContext
{
    public SECURITY_DBEntities()
        : base("name=SECURITY_DBEntities")
    {

    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        throw new UnintentionalCodeFirstException();
    }


    public virtual DbSet<UserMaster> UserMasters { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<trip> trips { get; set; }

}

}

