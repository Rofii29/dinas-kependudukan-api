' File: Program.vb (Versi Final dengan CORS)
Imports Microsoft.AspNetCore.Builder
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting

Public Module Program
    Public Sub Main(args As String())
        Dim builder = WebApplication.CreateBuilder(args)

        ' Menambahkan layanan CORS
        builder.Services.AddCors(Sub(options)
            options.AddDefaultPolicy(Sub(policy)
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
            End Sub)
        End Sub)

        builder.Services.AddControllers()
        Dim app = builder.Build()

        ' Menggunakan layanan CORS
        app.UseCors()
        
        app.MapControllers()
        app.Run()
    End Sub
End Module