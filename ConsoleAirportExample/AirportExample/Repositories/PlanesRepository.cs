using AirportExample.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using static AirportExample.Constants;

namespace AirportExample.Repositories;

public interface IPlanesRepository
{
    Plane? GetById (string planeId);
    Plane? Insert (Plane plane);
    Plane? Update (Plane plane);
    bool  Delete (string planeId);
}

public class PlanesRepository : IPlanesRepository
{       
    public Plane GetById(string planeId)
    {
       var command = "SELECT * FROM Aereo WHERE TipoAereo = @TipoAereo";
        return GetPlane(command, "@TipoAereo", planeId);
    }
    private Plane? GetPlane(string command, string parameterName, string value)
    {
        try
        {
            using var cn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand(command, cn);
            cn.Open();//per aprire la connessione
            cmd.Parameters.AddWithValue(parameterName, value);
            using var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleRow);
            if (reader?.Read() == true)
            {
                return new Plane()
                {
                    PlaneType = reader.GetString("TipoAereo"),
                    PassengerNumbers = reader.GetInt32("NumPasseggeri"),
                    CargoQuantity = reader.GetInt32("QtaMerci")
                };
            }
        }
        catch (SqlException ex)
        {
            Console.Error.WriteLine(ex);
        }
        return null;

    }

    public Plane? Insert(Plane plane)
    {
        var command = @"
            INSERT INTO [Aereo] ([TipoAereo],[NumPasseggeri],[QtaMerci])
            VALUES ( @TipoAereo, @NumPasseggeri,@QtaMerci )";
        var p = new Dictionary<string, object>()
        {
            { "@TipoAereo", plane.PlaneType },
            { "@NumPasseggeri", plane.PassengerNumbers},
            { "@QtaMerci", plane.CargoQuantity}
        };
        var rowsAffected = Execute(command, p);
        return rowsAffected > 0
            ? GetById(plane.PlaneType)
            : null;
    }

    public Plane? Update(Plane plane)
    {
        var command = @"
            UPDATE [Aereo] SET
            [NumPasseggeri] =  @NumPasseggeri,
            [QtaMerci] = @QtaMerci
            WHERE [TipoAereo] = @TipoAereo;";
        var p = new Dictionary<string, object>()
        {
            { "@TipoAereo", plane.PlaneType },
            { "@NumPasseggeri", plane.PassengerNumbers},
            { "@QtaMerci", plane.CargoQuantity}
        };
        var rowsAffected = Execute(command, p);
        return rowsAffected > 0
            ? GetById(plane.PlaneType)
            : null;
    }


    public bool Delete(string planeId)
    {
        var command = @"DELETE FROM [Aereo] WHERE [TipoAereo] = @TipoAereo;";
        var p = new Dictionary<string, object>()
        {
            { "@TipoAereo", planeId }
        };
        var rowsAffected = Execute(command, p);
        return rowsAffected.HasValue;
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