namespace ADSD.Backend.App.Json;

public class UserFullInfo : UserBaseInfo
{
    public string JobTitle { get; set; }
    public string Bio { get; set; }
    public string Nationality { get; set; }
    public string Document { get; set; }
    public bool IsLocal { get; set; }
    public bool Vip { get; set; }
    public bool NeedTransportation { get; set; }
    public string TransportComments { get; set; }
    public string AirlineName { get; set; }
    public string PlaneTicketNumber { get; set; }
    public string HotelBookingNumber { get; set; }
    public bool HotelCheckin { get; set; }
    public bool HotelCheckout { get; set; }
}