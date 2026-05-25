# 1. Aşama: Uygulamayı Derleme (SDK İmajı)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Tüm projeyi (src klasörünü ve altındaki iki projeyi de) içeri alıyoruz
COPY . ./

# Doğrudan API projesinin yolunu göstererek restore ve publish yapıyoruz
RUN dotnet restore src/NlpPipeline.Api/NlpPipeline.Api.csproj
RUN dotnet publish src/NlpPipeline.Api/NlpPipeline.Api.csproj -c Release -o out

# 2. Aşama: Uygulamayı Çalıştırma (Hafif Runtime İmajı)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 8080

# Gerçek DLL adımızı tam olarak tanımlıyoruz
ENTRYPOINT ["dotnet", "NlpPipeline.Api.dll"]