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
    
    public partial class Persons
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Persons()
        {
            this.Booking_Persons = new HashSet<Booking_Persons>();
        }
    
        public int id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public System.DateTime Birthdate { get; set; }
        public long Passport_number { get; set; }
        public int Passport_Country { get; set; }
        public string CountryName
        {
            get
            {
                return Countries.Country;
            }
        }
        public long Phone { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Booking_Persons> Booking_Persons { get; set; }
        public virtual Countries Countries { get; set; }
    }
}
