# 1. Aşama: Uygulamayı Derleme (SDK İmajı)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Önce projedeki her şeyi Docker imajının içine kopyalıyoruz
COPY . ./

# Klasör bağımsız, bulunan projeyi restore et ve Release modunda publish et
RUN dotnet restore
RUN dotnet publish -c Release -o out

# 2. Aşama: Uygulamayı Çalıştırma (Hafif Runtime İmajı)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Konteyner içi çalışma portu
EXPOSE 8080

ENTRYPOINT ["dotnet", "Natural-Language-ProgramingAppDevOps.dll"]