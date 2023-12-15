INSERT INTO GE_PARAMETRO (ID_PARAMETRO, DESCRIPCION, VALOR, AGRUPADOR, CLAVE)
SELECT MAX(ID_PARAMETRO) + 1, 'Habilita o deshabilita la Interfase con el Sistema Tributario', '1', 'INTERFASE_SISTEMA_TRIBUTARIO', 'IST_ENABLED'  
FROM GE_PARAMETRO;

INSERT INTO GE_PARAMETRO (ID_PARAMETRO, DESCRIPCION, VALOR, AGRUPADOR, CLAVE)
SELECT MAX(ID_PARAMETRO) + 1, 'Webservice de Avaluo del Sistema Tributario', 
    'http://emaw70512.hopto.org/samws/inmAvaluo.php', 
    'INTERFASE_SISTEMA_TRIBUTARIO', 'IST_WS_AVALUO'  
FROM GE_PARAMETRO;
               
INSERT INTO GE_PARAMETRO (ID_PARAMETRO, DESCRIPCION, VALOR, AGRUPADOR, CLAVE)
SELECT MAX(ID_PARAMETRO) + 1, 'Webservice de Avaluo Masivo del Sistema Tributario', 
    'http://emaw70512.hopto.org/samws/inmAvaluoMasivo.php', 
    'INTERFASE_SISTEMA_TRIBUTARIO', 'IST_WS_AVALUO_MASIVO'  
FROM GE_PARAMETRO;
                                                
INSERT INTO GE_PARAMETRO (ID_PARAMETRO, DESCRIPCION, VALOR, AGRUPADOR, CLAVE)
SELECT MAX(ID_PARAMETRO) + 1, 'Webservice de Baja del Sistema Tributario', 
    'http://emaw70512.hopto.org/samws/inmBaja.php', 
    'INTERFASE_SISTEMA_TRIBUTARIO', 'IST_WS_BAJA'  
FROM GE_PARAMETRO;                         
           
INSERT INTO GE_PARAMETRO (ID_PARAMETRO, DESCRIPCION, VALOR, AGRUPADOR, CLAVE)
SELECT MAX(ID_PARAMETRO) + 1, 'Webservice de Verificación de Deudas del Sistema Tributario', 
    'http://emaw70512.hopto.org/samws/inmBajaPedido.php', 
    'INTERFASE_SISTEMA_TRIBUTARIO', 'IST_WS_BAJA_PEDIDO'  
FROM GE_PARAMETRO;          
            
INSERT INTO GE_PARAMETRO (ID_PARAMETRO, DESCRIPCION, VALOR, AGRUPADOR, CLAVE)
SELECT MAX(ID_PARAMETRO) + 1, 'Webservice de Edición del Sistema Tributario', 
    'http://emaw70512.hopto.org/samws/inmEdita.php', 
    'INTERFASE_SISTEMA_TRIBUTARIO', 'IST_WS_EDICION'  
FROM GE_PARAMETRO;

COMMIT;