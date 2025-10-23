' File: Program.vb (Versi Final yang Sudah Diperbaiki)

Imports Microsoft.AspNetCore.Builder
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting

Public Module Program
    Public Sub Main(args As String())
        Dim builder = WebApplication.CreateBuilder(args)

        ' Menambahkan layanan untuk Controller API
        builder.Services.AddControllers()

        ' Membangun aplikasi
        Dim app = builder.Build()

        ' Mengarahkan request ke Controller
        app.MapControllers()

        ' Menjalankan aplikasi
        app.Run()
    End Sub
End Module