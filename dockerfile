# FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
# WORKDIR /App

# # Copy everything
# COPY . ./
# # Restore as distinct layers
# RUN dotnet restore
# # Build and publish a release
# RUN dotnet publish -c Release -o out

# # Build runtime image
# FROM mcr.microsoft.com/dotnet/aspnet:7.0
# WORKDIR /App
# COPY --from=build-env /App/out .
# ENTRYPOINT ["dotnet", "DotNet.Docker.dll"]
# Set the base image to use for building your application


FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Download and install LibreOffice
# ADD https://downloadarchive.documentfoundation.org/libreoffice/old/6.4.7.2/win/x86_64/LibreOffice_6.4.7.2_Win_x64.msi /libreoffice.msi
# RUN Start-Process -Wait -FilePath "msiexec" -ArgumentList "/i libreoffice.msi /quiet /norestart"
# RUN Remove-Item -Force libreoffice.msi

RUN apt-get update

RUN apt-get install wget libgdiplus -y

RUN wget -P /app https://github.com/rdvojmoc/DinkToPdf/raw/master/v0.12.4/64%20bit/libwkhtmltox.dll

RUN wget -P /app https://github.com/rdvojmoc/DinkToPdf/raw/master/v0.12.4/64%20bit/libwkhtmltox.dylib

RUN wget -P /app https://github.com/rdvojmoc/DinkToPdf/raw/master/v0.12.4/64%20bit/libwkhtmltox.so

WORKDIR /app

# Copy the .csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore --disable-parallel

# Copy the rest of the application source code and build
COPY . ./
RUN dotnet publish -c Release -o out

# Set the base image to use for running your application
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

WORKDIR /app

# Copy the built application from the previous stage
COPY --from=build /app/out ./

# Expose the port that your ASP.NET Core application listens on
EXPOSE 80

# Set the entry point for your application
ENTRYPOINT ["dotnet", "Recruitment_portal.dll"]
