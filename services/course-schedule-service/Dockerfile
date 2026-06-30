FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Lệnh này sẽ tìm tất cả file .csproj trong thư mục hiện tại và các thư mục con
COPY . .
RUN dotnet restore "N1.csproj"

RUN dotnet build "N1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "N1.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Ép ứng dụng chạy ở port 8080 để Railway nhận diện được
ENTRYPOINT ["dotnet", "N1.dll", "--urls", "http://0.0.0.0:8080"]
