CREATE OR REPLACE TRIGGER LOG_IF_RENTAS_TR 
BEFORE INSERT ON LOG_IF_RENTAS 
REFERENCING NEW AS new OLD AS old
FOR EACH ROW
BEGIN
   SELECT LOG_IF_RENTAS_SEQ.NEXTVAL INTO :new.ID_LOG FROM DUAL;
END;
