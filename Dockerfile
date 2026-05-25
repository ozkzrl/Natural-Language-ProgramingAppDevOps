# 1. Aşama: Uygulamayı Derleme (SDK İmajı)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Tüm projeyi (src dahil) imajın içine kopyalıyoruz
COPY . ./

# Proje dosyaları src klasöründe olduğu için o dizine odaklanarak restore ve publish yapıyoruz
RUN dotnet restore src/*.csproj
RUN dotnet publish src/*.csproj -c Release -o out

# 2. Aşama: Uygulamayı Çalıştırma (Hafif Runtime İmajı)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Konteyner içi çalışma portu
EXPOSE 8080

ENTRYPOINT ["dotnet", "Natural-Language-ProgramingAppDevOps.dll"]