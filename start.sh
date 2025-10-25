cd ./product_catalog_service || exit
dotnet watch run
cd ../inventory_service || exit
dotnet watch run
cd ../order_service || exit
mvn spring-boot:run
cd ../payment_service || exit
npm run dev
