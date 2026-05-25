# 1. Aşama: Çalışma Ortamı (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# 2. Aşama: SDK ile Derleme Ortamı
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# src altındaki tüm katmanları Docker içerisine kopyalıyoruz
COPY ["src/", "src/"]

# Web API projesinin NuGet paketlerini tam klasör yolundan geri yüklüyoruz (Restore)
RUN dotnet restore "src/NlpPipeline.Api/NlpPipeline.Api.csproj"

# Projeyi Release modunda derliyoruz
RUN dotnet build "src/NlpPipeline.Api/NlpPipeline.Api.csproj" -c Release -o /app/build

# 3. Aşama: Uygulamayı Yayınlama (Publish)
FROM build AS publish
RUN dotnet publish "src/NlpPipeline.Api/NlpPipeline.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Aşama: Son Hafif İmajı Hazırlama
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Çalıştırılacak ana Web API DLL dosyanızın tam adı:
ENTRYPOINT ["dotnet", "NlpPipeline.Api.dll"]