@echo off
@echo Inserting into db:
sqlcmd -U sa -P ikrzikrz -S loaner\sqlexpress -d klick_vending_machine -q "exit(insert into CardScan (cardid, ScanDate) values ('%1', getdate()))" 
@echo Logging to C:\Users\Administrator\Documents\GitHub\Vending-Machine\WindowsApp\scans.log
echo %date% %time% %1 >> C:\Users\Administrator\Documents\GitHub\Vending-Machine\WindowsApp\scans.log
pause