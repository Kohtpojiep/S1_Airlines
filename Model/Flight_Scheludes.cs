//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace S1_Вариант3.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Flight_Scheludes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Flight_Scheludes()
        {
            this.Bookings = new HashSet<Bookings>();
        }
    
        public int id { get; set; }
        public System.DateTime Date { get; set; }
        public System.TimeSpan Time { get; set; }
        public int CountryFromId { get; set; }
        public int CountryToId { get; set; }
        public short FlightNumber { get; set; }
        public int AircraftId { get; set; }
        public decimal EconomyPrice { get; set; }
        public short BusinessPrice
        {
            get
            {
                return Convert.ToInt16((Double)EconomyPrice * 1.35);
            }
        }
        public short FirstClassPrice
        {
            get
            {
                return Convert.ToInt16((Double)EconomyPrice * 1.35 * 1.30);
            }
        }
        public string GetShortCountryFrom
        {
            get
            {
                return Countries.Country;
            }
        }
        public string GetShortCountryTo
        {
            get
            {
                return Countries1.Country;
            }
        }
        public bool IsAccepted { get; set; }
    
        public virtual Aircraft Aircraft { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bookings> Bookings { get; set; }
        public virtual Countries Countries { get; set; }
        public virtual Countries Countries1 { get; set; }
    }
}
