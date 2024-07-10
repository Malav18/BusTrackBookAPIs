public class Driver
{
    public int DriverId { get; set; }
    public string DriverName { get; set; }
    public string PhoneNumber { get; set; }
    public string DrivingLicenseNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string AddressStreet { get; set; }
    public string AddressCity { get; set; }
    public string AddressState { get; set; }
    public string AddressZipCode { get; set; }
    public string EmergencyContact { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
