FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY RaceDay.Mvc/RaceDay.Mvc.csproj RaceDay.Mvc/
RUN dotnet restore RaceDay.Mvc/RaceDay.Mvc.csproj
COPY RaceDay.Mvc/ RaceDay.Mvc/
RUN dotnet publish RaceDay.Mvc/RaceDay.Mvc.csproj -c Release -o /app/publish --no-restore
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "RaceDay.Mvc.dll"]