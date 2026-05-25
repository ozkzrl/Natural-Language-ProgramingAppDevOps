# 1. Aşama: Uygulamayı Derleme (SDK İmajı)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# NuGet paketlerini kopyala ve restore et
COPY *.sln ./
COPY *.csproj ./
RUN dotnet restore

# Tüm proje dosyalarını kopyala ve yayın klasörünü oluştur (Publish)
COPY . ./
RUN dotnet publish -c Release -o out

# 2. Aşama: Uygulamayı Çalıştırma (Hafif Runtime İmajı)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Konteyner içi çalışma portunu belirle
EXPOSE 8080

ENTRYPOINT ["dotnet", "Natural-Language-ProgramingAppDevOps.dll"]