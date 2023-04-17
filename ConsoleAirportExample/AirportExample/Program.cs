
using AirportExample.Models;
using AirportExample.Repositories;
using AirportExample.Services;
Console.WriteLine("Hello");
//new ServiceMenu().Start();

var flightsRepository = new FlightsRepository();
            var airportsRepository = new AirportsRepository();
            var planesRepository = new PlanesRepository();

            var flightId = "AZ274";
            var flight = flightsRepository.GetById(flightId);

            if (flight != null)
            {
                flight.Departure = airportsRepository.GetById(flight.Departure.City);
                flight.Arrival = airportsRepository.GetById(flight.Arrival.City);
                flight.Plane = planesRepository.GetById(flight.Plane.PlaneType);

                Console.WriteLine($"Volo {flight.Id} - {flight.WeekDay}");
                Console.WriteLine($"Partenza: {flight.Departure.City}  alle {flight.DepartureTime}");
                Console.WriteLine($"Arrivo: {flight.Arrival.City} alle {flight.ArrivalTime}");
                Console.WriteLine($"Aereo: {flight.Plane.PlaneType} ");
            }
            else
            {
                Console.WriteLine($"Volo con ID {flightId} non trovato.");
            }
Console.WriteLine("Bye");
