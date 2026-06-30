FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
# Railway/Render sẽ tự cung cấp PORT qua biến môi trường
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["N1.csproj", "."]
RUN dotnet restore "N1.csproj"
COPY . .
RUN dotnet build "N1.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "N1.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Lệnh khởi chạy và ép ứng dụng nghe theo Port của Cloud
ENTRYPOINT ["dotnet", "N1.dll", "--urls", "http://0.0.0.0:8080"]
