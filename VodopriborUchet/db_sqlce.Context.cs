//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VodopriborUchet
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class db_sqlceEntities : DbContext
    {
        public db_sqlceEntities()
            : base("name=db_sqlceEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<category_pay_type> category_pay_type { get; set; }
        public virtual DbSet<counters> counters { get; set; }
        public virtual DbSet<counters_type> counters_type { get; set; }
        public virtual DbSet<net> net { get; set; }
        public virtual DbSet<object_type> object_type { get; set; }
        public virtual DbSet<objects_place> objects_place { get; set; }
        public virtual DbSet<owners> owners { get; set; }
        public virtual DbSet<recorder_type> recorder_type { get; set; }
        public virtual DbSet<resource_type> resource_type { get; set; }
        public virtual DbSet<units> units { get; set; }
        public virtual DbSet<users> users { get; set; }
        public virtual DbSet<users_type> users_type { get; set; }
    }
}