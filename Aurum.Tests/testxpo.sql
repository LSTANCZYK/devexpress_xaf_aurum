begin
  execute immediate 'drop schema testxpo';
exception when others then
  null;
end;
/
begin
  execute immediate 'drop user testxpo cascade';
exception when others then
  null;
end;
/
create user testxpo identified by testxpo123;
grant unlimited tablespace to testxpo;
grant create session to testxpo;

create table testxpo.class1 (
  id number not null,
  field1 varchar2(20),
  field2 varchar2(20)
);
