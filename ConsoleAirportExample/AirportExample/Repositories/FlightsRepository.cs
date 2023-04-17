using AirportExample.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using static AirportExample.Constants;

namespace AirportExample.Repositories;

public interface IFlightsRepository
{
     Flight? GetById (string flightId);
     Flight? Insert (Flight flight);
     Flight? Update (Flight flight);
     Flight? Delete (string flightId);
}
public class FlightsRepository : IFlightsRepository
{

    public Flight? GetById(string flightId)
    {
        var command = "SELECT * FROM Volo WHERE IdVolo = @IdVolo";
        return GetFlight(command, "@IdVolo", flightId);
    }

    private Flight? GetFlight(string command, string parameterName, string value)
    {
        try
        {
            using var cn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand(command, cn);
            cn.Open();
            cmd.Parameters.AddWithValue(parameterName, value);
            using var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleRow);
            if (reader?.Read() == true)
            {
                return new Flight()
                {
                    Id = reader.GetString("IdVolo"),
                    WeekDay = reader.GetString("GiornoSett"),
                    Departure = new Airport()
                    {
                        City = reader.GetString("CittaPart")
                    },
                    Arrival = new Airport()
                    {
                        City = reader.GetString("CittaArr")
                    },
                    //qui mi dice che non e possibili convertire da string a int
                    //DepartureTime = reader.GetTimeSpan("OraPart"),
                    //ArrivalTime = reader.GetTimeSpan("OraArr"),
                    Plane = new Plane()
                    {
                        PlaneType = reader.GetString("TipoAereo")
                    }
                };
            }
        }
        catch (SqlException ex)
        {
            Console.Error.WriteLine(ex);
        }
        return null;
    }

    public Flight? Insert(Flight flight)
    {
        var command = @"
            INSERT INTO [Volo] ([IdVolo],[GiornoSett],[CittaPart],[OraPart],[CittaArr],[OraArr],[TipoAereo])
            VALUES ( @IdVolo , @GiornoSett , @CittaPart , @OraPart , @CittaArr , @OraArr , @TipoAereo )";
        var p = new Dictionary<string, object>()
        {
            {"@IdVolo", flight.Id },
            {"@GiornoSett", flight.WeekDay },
            {"@CittaPart", flight.Departure.City },
            {"@OraPart", flight.DepartureTime },
            {"@CittaArr", flight.Arrival.City },
            {"@OraArr", flight.ArrivalTime },
            {"@TipoAereo", flight.Plane.PlaneType }
        };
        var rowsAffected = Execute(command, p);
        return rowsAffected > 0
            ? GetById(flight.Id)
            : null;
    }
    public Flight? Update(Flight flight)
    {
        var command = @"
            UPDATE [Volo]
            SET 
            [GiornoSett] = @GiornoSett,
            [CittaPart] = @CittaPart,
            [OraPart] = @OraPart,
            [CittaArr] = @CittaArr,
            [OraArr] = @OraArr,
            [TipoAereo] = @TipoAereo
            WHERE [IdVolo] = @IdVolo";
        var p = new Dictionary<string, object>()
        {
            {"@IdVolo", flight.Id },
            {"@GiornoSett", flight.WeekDay },
            {"@CittaPart", flight.Departure.City },
            {"@OraPart", flight.DepartureTime },
            {"@CittaArr", flight.Arrival.City },
            {"@OraArr", flight.ArrivalTime },
            {"@TipoAereo", flight.Plane.PlaneType }
        };
        var rowsAffected = Execute(command, p);
        return rowsAffected > 0
            ? GetById(flight.Id)
            : null;
    }

    public Flight? Delete(string flightId)
    {
        var deletedFlight = GetById(flightId);
        if (deletedFlight == null) return null;
        var command = @"DELETE FROM [Volo] WHERE [IdVolo] = @IdVolo";
        var p = new Dictionary<string, object>()
        {
            {"@IdVolo", flightId }
        };
        var rowsAffected = Execute(command, p);
        return rowsAffected > 0 ? deletedFlight : null;
    }

    private int? Execute(string command, Dictionary<string, object> parameters)
    {
        try
        {
            using var cn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand(command, cn);
            cn.Open();
            var p = parameters.Select(x => new SqlParameter(x.Key, x.Value)).ToArray();
            cmd.Parameters.AddRange(p);
            return cmd.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            Console.Error.WriteLine(ex);
        }
        return null;
    }
}