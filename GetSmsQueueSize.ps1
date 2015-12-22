$proxy = New-WebServiceProxy -Uri http://localhost:62572/OnCellWebService.svc?wsdl
$proxy.GetSmsQueueSize()