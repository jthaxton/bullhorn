# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app
# ENV ASPNETCORE_URLS="http://0.0.0.0:5200"

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out
# EXPOSE 5200
# EXPOSE 7245
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
# RUN dotnet dev-certs https --clean
# RUN dotnet dev-certs https
ENTRYPOINT ["dotnet", "bullhorn.dll"]