dotnet build

dotnet test # tất cả
dotnet test --filter "FullyQualifiedName~DataDrivenLoginTests" # 1 class
dotnet test --filter "Name=DangNhap_01_standard_user" # 1 ca
dotnet test -l "console;verbosity=detailed" # thấy Console.WriteLine

TEST_ENV=staging dotnet test # đổi môi trường, không sửa file
TEST_Browser=firefox dotnet test # đè config từ dòng lệnh (hữu ích cho CI)
