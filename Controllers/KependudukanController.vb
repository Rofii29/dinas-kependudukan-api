' File: Controllers/KependudukanController.vb (Versi Final dengan Nama_pemohon)
Imports System.Data.OleDb
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.Extensions.Configuration
Imports dinas_kependudukan_api.Models

' ** Model Bantuan untuk Menggabungkan Data **
Public Class HasilCekLayanan
    ' Properti dari tabel PengajuanLayanan
    Public Property ID_Layanan As String
    Public Property Nama_pemohon As String   ' <-- TAMBAHKAN PROPERTI BARU DI SINI
    Public Property Jenis_Layanan As String
    Public Property Tanggal_Pengajuan As Date
    Public Property Status_Layanan As String

    ' Properti ini akan berisi daftar dari tabel StatusPersyaratan
    Public Property Status_Persyaratan As List(Of StatusPersyaratan)
End Class


<Route("api/[controller]")>
<ApiController>
Public Class KependudukanController
    Inherits ControllerBase

    Private ReadOnly _connectionString As String

    Public Sub New(ByVal configuration As IConfiguration)
        _connectionString = configuration.GetConnectionString("AccessDb")
    End Sub

    ' --- Endpoint untuk mengambil SEMUA data ---
    <HttpGet("status")>
    Public Function GetAllStatusLayanan() As ActionResult(Of List(Of HasilCekLayanan))
        Dim daftarHasil As New List(Of HasilCekLayanan)()

        Using connection As New OleDbConnection(_connectionString)
            Try
                connection.Open()

                Dim daftarPengajuan As New List(Of PengajuanLayanan)()
                Dim sqlSemuaLayanan As String = "SELECT * FROM PengajuanLayanan ORDER BY ID_Pengajuan ASC"
                Using cmdLayanan As New OleDbCommand(sqlSemuaLayanan, connection)
                    Using reader As OleDbDataReader = cmdLayanan.ExecuteReader()
                        While reader.Read()
                            daftarPengajuan.Add(New PengajuanLayanan With {
                                .ID_Pengajuan = reader("ID_Pengajuan").ToString(),
                                .Nama_pemohon = reader("Nama_pemohon").ToString(), ' <-- BACA KOLOM BARU DARI DATABASE
                                .Jenis_Layanan = reader("Jenis_Layanan").ToString(),
                                .Tanggal_Pengajuan = Convert.ToDateTime(reader("Tanggal_Pengajuan")),
                                .Status_Layanan = reader("Status_Layanan").ToString()
                            })
                        End While
                    End Using
                End Using

                For Each pengajuan In daftarPengajuan
                    Dim hasil As New HasilCekLayanan With {
                        .ID_Layanan = pengajuan.ID_Pengajuan,
                        .Nama_pemohon = pengajuan.Nama_pemohon, ' <-- MASUKKAN DATA BARU KE HASIL
                        .Jenis_Layanan = pengajuan.Jenis_Layanan,
                        .Tanggal_Pengajuan = pengajuan.Tanggal_Pengajuan,
                        .Status_Layanan = pengajuan.Status_Layanan
                    }
                    
                    Dim persyaratanList As New List(Of StatusPersyaratan)()
                    Dim sqlPersyaratan As String = "SELECT * FROM StatusPersyaratan WHERE ID_Pengajuan = @id"
                    Using cmdPersyaratan As New OleDbCommand(sqlPersyaratan, connection)
                        cmdPersyaratan.Parameters.AddWithValue("@id", pengajuan.ID_Pengajuan)
                        Using reader As OleDbDataReader = cmdPersyaratan.ExecuteReader()
                            While reader.Read()
                                persyaratanList.Add(New StatusPersyaratan With {
                                    .ID = Convert.ToInt32(reader("ID")),
                                    .Nama_Persyaratan = reader("Nama_Persyaratan").ToString(),
                                    .Status_Kelengkapan = reader("Status_Kelengkapan").ToString(),
                                    .ID_Pengajuan = reader("ID_Pengajuan").ToString()
                                })
                            End While
                        End Using
                    End Using
                    
                    hasil.Status_Persyaratan = persyaratanList
                    daftarHasil.Add(hasil)
                Next

                Return Ok(daftarHasil)
            Catch ex As Exception
                Return StatusCode(500, "Server Error: " & ex.Message)
            End Try
        End Using
    End Function

    ' --- Endpoint untuk mengambil SATU data spesifik ---
    <HttpGet("status/{idPengajuan}")>
    Public Function GetStatusLayananById(ByVal idPengajuan As String) As ActionResult(Of HasilCekLayanan)
        Dim hasil As New HasilCekLayanan()
        Dim persyaratanList As New List(Of StatusPersyaratan)()
        Using connection As New OleDbConnection(_connectionString)
            Try
                connection.Open()

                Dim sqlLayanan As String = "SELECT * FROM PengajuanLayanan WHERE ID_Pengajuan = @id"
                Using cmdLayanan As New OleDbCommand(sqlLayanan, connection)
                    cmdLayanan.Parameters.AddWithValue("@id", idPengajuan)
                    Using reader As OleDbDataReader = cmdLayanan.ExecuteReader()
                        If reader.Read() Then
                            hasil.ID_Layanan = reader("ID_Pengajuan").ToString()
                            hasil.Nama_pemohon = reader("Nama_pemohon").ToString() ' <-- BACA KOLOM BARU DARI DATABASE
                            hasil.Jenis_Layanan = reader("Jenis_Layanan").ToString()
                            hasil.Tanggal_Pengajuan = Convert.ToDateTime(reader("Tanggal_Pengajuan"))
                            hasil.Status_Layanan = reader("Status_Layanan").ToString()
                        Else
                            Return NotFound($"Pengajuan dengan ID '{idPengajuan}' tidak ditemukan.")
                        End If
                    End Using
                End Using

                Dim sqlPersyaratan As String = "SELECT * FROM StatusPersyaratan WHERE ID_Pengajuan = @id"
                Using cmdPersyaratan As New OleDbCommand(sqlPersyaratan, connection)
                    cmdPersyaratan.Parameters.AddWithValue("@id", idPengajuan)
                    Using reader As OleDbDataReader = cmdPersyaratan.ExecuteReader()
                        While reader.Read()
                            persyaratanList.Add(New StatusPersyaratan With {
                                .ID = Convert.ToInt32(reader("ID")),
                                .Nama_Persyaratan = reader("Nama_Persyaratan").ToString(),
                                .Status_Kelengkapan = reader("Status_Kelengkapan").ToString(),
                                .ID_Pengajuan = reader("ID_Pengajuan").ToString()
                            })
                        End While
                    End Using
                End Using

                hasil.Status_Persyaratan = persyaratanList
                Return Ok(hasil)
            Catch ex As Exception
                Return StatusCode(500, "Server Error: " & ex.Message)
            End Try
        End Using
    End Function
End Class