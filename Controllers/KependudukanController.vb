Imports System.Data.OleDb
Imports Microsoft.AspNetCore.Mvc
Imports dinas_kependudukan_api.Models ' Pastikan namespace ini cocok dengan RootNamespace di .vbproj
Imports Microsoft.Extensions.Configuration
<Route("api/[controller]")>
<ApiController>
Public Class KependudukanController
    Inherits ControllerBase

    Private ReadOnly _connectionString As String

    Public Sub New(ByVal configuration As IConfiguration)
        _connectionString = configuration.GetConnectionString("AccessDb")
    End Sub

    <HttpGet("layanan")>
    Public Function GetLayanan() As ActionResult(Of List(Of Layanan))
        Dim layananList As New List(Of Layanan)()
        Using connection As New OleDbConnection(_connectionString)
            Try
                connection.Open()
                Dim sqlQuery As String = "SELECT * FROM Layanan"
                Using command As New OleDbCommand(sqlQuery, connection)
                    Using reader As OleDbDataReader = command.ExecuteReader()
                        While reader.Read()
                            layananList.Add(New Layanan With {
                                .ID_layanan = Convert.ToInt32(reader("ID_layanan")),
                                .nama_layanan = reader("nama_layanan").ToString(),
                                .deskripsi = reader("deskripsi").ToString()
                            })
                        End While
                    End Using
                End Using
                Return Ok(layananList)
            Catch ex As Exception
                Return StatusCode(500, "Server Error: " & ex.Message)
            End Try
        End Using
    End Function

    <HttpGet("layanan/{idLayanan}/persyaratan")>
    Public Function GetPersyaratanByLayananId(ByVal idLayanan As Integer) As ActionResult(Of List(Of Persyaratan))
        Dim persyaratanList As New List(Of Persyaratan)()
        Using connection As New OleDbConnection(_connectionString)
            Try
                connection.Open()
                Dim sqlQuery As String = "SELECT * FROM Persyaratan WHERE id_layanan = @id"
                Using command As New OleDbCommand(sqlQuery, connection)
                    command.Parameters.AddWithValue("@id", idLayanan)
                    Using reader As OleDbDataReader = command.ExecuteReader()
                        While reader.Read()
                            persyaratanList.Add(New Persyaratan With {
                                .ID_persyaratan = Convert.ToInt32(reader("ID_persyaratan")),
                                .nama_persyaratan = reader("nama_persyaratan").ToString(),
                                .isi_persyaratan = reader("isi_persyaratan").ToString(),
                                .id_layanan = Convert.ToInt32(reader("id_layanan"))
                            })
                        End While
                    End Using
                End Using
                If persyaratanList.Count = 0 Then
                    Return NotFound($"No requirements found for Layanan ID {idLayanan}")
                End If
                Return Ok(persyaratanList)
            Catch ex As Exception
                Return StatusCode(500, "Server Error: " & ex.Message)
            End Try
        End Using
    End Function
End Class