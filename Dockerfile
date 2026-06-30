FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy tất cả các file vào container
COPY . .

# Tự động tìm bất kỳ file .csproj nào để restore và build
RUN dotnet restore $(ls *.csproj)
RUN dotnet build $(ls *.csproj) -c Release -o /app/build

FROM build AS publish
RUN dotnet publish $(ls *.csproj) -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Tìm file .dll chính để chạy (Giả sử file dll cùng tên với project)
ENTRYPOINT ["sh", "-c", "dotnet $(ls *.dll | head -n 1) --urls http://0.0.0.0:8080"]
