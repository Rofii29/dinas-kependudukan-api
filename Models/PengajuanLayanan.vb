' File: Models/PengajuanLayanan.vb
Namespace Models
    Public Class PengajuanLayanan
        Public Property ID_Pengajuan As String
        Public Property Nama_pemohon As String  ' <-- TAMBAHKAN BARIS INI
        Public Property Jenis_Layanan As String
        Public Property Tanggal_Pengajuan As Date
        Public Property Status_Layanan As String
    End Class
End Namespace