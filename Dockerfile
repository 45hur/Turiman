FROM mcr.microsoft.com/dotnet/sdk:5.0.102-ca-patch-buster-slim AS build

RUN apt-get update
RUN apt-get install -qq 
WORKDIR /src
COPY ["Turiman/Turiman.csproj", "/src"]
# RUN dotnet restore "Turiman.csproj"
COPY Turiman /src

FROM build AS publish

WORKDIR /src
RUN dotnet publish -c Release -r linux-x64 -o /app/publish --self-contained true /p:PublishTrimmed=true

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS runtime
RUN apt-get update && apt-get install -qq  && \
    apt-get clean autoclean && apt-get autoremove --yes && rm -rf /var/lib/{apt,dpkg,cache,log}/

WORKDIR /app

COPY --from=publish /app/publish ./

ENTRYPOINT ["dotnet", "Turiman.dll"] 