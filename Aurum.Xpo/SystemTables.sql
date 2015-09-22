create table %user%.moduleinfo (
  id int not null,
  version nvarchar2(100),
  name nvarchar2(100),
  assemblyfilename nvarchar2(100),
  ismain number(1,0),
  optimisticlockfield int
);
create sequence %user%.seq_moduleinfo start with 1 increment by 1 nocache;
alter table %user%.moduleinfo add constraint pk_moduleinfo primary key (id);
create or replace view %user%.vw_moduleinfo as 
select id,version,name,assemblyfilename,ismain,optimisticlockfield from %user%.moduleinfo 
with read only;

create or replace package %user%.pkg_moduleinfo as
-- Autocreation

procedure Add(
  pID out int,
  pVersion nvarchar2,
  pName nvarchar2,
  pAssemblyFileName nvarchar2,
  pIsMain number,
  pOptimisticLockField int
);

procedure Edit(
  pID int,
  pVersion nvarchar2,
  pName nvarchar2,
  pAssemblyFileName nvarchar2,
  pIsMain number,
  pOptimisticLockField int,
  SetValueFlags number := 31
);

procedure Del(
  pID int
);

end;
/

create or replace package body %user%.pkg_moduleinfo as

procedure Add(
  pID out int,
  pVersion nvarchar2,
  pName nvarchar2,
  pAssemblyFileName nvarchar2,
  pIsMain number,
  pOptimisticLockField int
) as
begin
  insert into moduleinfo (id, version, name, assemblyfilename, ismain, optimisticlockfield)
  values(seq_moduleinfo.nextval, pVersion, pName, pAssemblyFileName, pIsMain, pOptimisticLockField)
  returning id into pID;
end;

procedure Edit(
  pID int,
  pVersion nvarchar2,
  pName nvarchar2,
  pAssemblyFileName nvarchar2,
  pIsMain number,
  pOptimisticLockField int,
  SetValueFlags number := 31
) as
begin
  update moduleinfo set
    version = decode(bitand(1,SetValueFlags),1,pVersion,version),
    name = decode(bitand(2,SetValueFlags),2,pName,name),
    assemblyfilename = decode(bitand(4,SetValueFlags),4,pAssemblyFileName,assemblyfilename),
    ismain = decode(bitand(8,SetValueFlags),8,pIsMain,ismain),
    optimisticlockfield = decode(bitand(16,SetValueFlags),16,pOptimisticLockField,optimisticlockfield)
  where id = pID;
  if sql%rowcount <> 1 then 
    raise_application_error(-20001, 'Record moduleinfo('||pID||') is not found');
  end if;
end;

procedure Del(
  pID int
) as
begin
  delete from moduleinfo where id = pID;
  if sql%rowcount <> 1 then 
    raise_application_error(-20001, 'Record moduleinfo('||pID||') is not found');
  end if;
end;

end;
/

create or replace procedure %user%.p_moduleinfo_a(
  pID out int,
  pVersion nvarchar2,
  pName nvarchar2,
  pAssemblyFileName nvarchar2,
  pIsMain number,
  pOptimisticLockField int
) 
as begin pkg_moduleinfo.Add(pID, pVersion, pName, pAssemblyFileName, pIsMain, pOptimisticLockField); end;
/
create or replace procedure %user%.p_moduleinfo_e(
  pID int,
  pVersion nvarchar2,
  pName nvarchar2,
  pAssemblyFileName nvarchar2,
  pIsMain number,
  pOptimisticLockField int,
  SetValueFlags number := 31
) 
as begin pkg_moduleinfo.Edit(pID, pVersion, pName, pAssemblyFileName, pIsMain, pOptimisticLockField, SetValueFlags); end;
/
create or replace procedure %user%.p_moduleinfo_d(
  pID int
) 
as begin pkg_moduleinfo.Del(pID); end;
/

create table %user%.xpobjecttype (
  oid int not null,
  typename nvarchar2(254),
  assemblyname nvarchar2(254)
);
create sequence %user%.seq_xpobjecttype start with 1 increment by 1 nocache;
alter table %user%.xpobjecttype add constraint pk_xpobjecttype primary key (oid);
create unique index %user%.idx_xpobjecttypetypename on %user%.xpobjecttype(typename);

create or replace view %user%.vw_xpobjecttype as 
select oid,typename,assemblyname from %user%.xpobjecttype 
with read only;

create or replace package %user%.pkg_xpobjecttype as
-- Autocreation

procedure Add(
  pOID out int,
  pTypeName nvarchar2,
  pAssemblyName nvarchar2
);

procedure Edit(
  pOID int,
  pTypeName nvarchar2,
  pAssemblyName nvarchar2,
  SetValueFlags number := 3
);

procedure Del(
  pOID int
);

end;
/

create or replace package body %user%.pkg_xpobjecttype as

procedure Add(
  pOID out int,
  pTypeName nvarchar2,
  pAssemblyName nvarchar2
) as
begin
  insert into xpobjecttype (oid, typename, assemblyname)
  values(seq_xpobjecttype.nextval, pTypeName, pAssemblyName)
  returning oid into pOID;
end;

procedure Edit(
  pOID int,
  pTypeName nvarchar2,
  pAssemblyName nvarchar2,
  SetValueFlags number := 3
) as
begin
  update xpobjecttype set
    typename = decode(bitand(1,SetValueFlags),1,pTypeName,typename),
    assemblyname = decode(bitand(2,SetValueFlags),2,pAssemblyName,assemblyname)
  where oid = pOID;
  if sql%rowcount <> 1 then 
    raise_application_error(-20001, 'Record xpobjecttype('||pOID||') is not found');
  end if;
end;

procedure Del(
  pOID int
) as
begin
  delete from xpobjecttype where oid = pOID;
  if sql%rowcount <> 1 then 
    raise_application_error(-20001, 'Record xpobjecttype('||pOID||') is not found');
  end if;
end;

end;
/

create or replace procedure %user%.p_xpobjecttype_a(
  pOID out int,
  pTypeName nvarchar2,
  pAssemblyName nvarchar2
) 
as begin pkg_xpobjecttype.Add(pOID, pTypeName, pAssemblyName); end;
/
create or replace procedure %user%.p_xpobjecttype_e(
  pOID int,
  pTypeName nvarchar2,
  pAssemblyName nvarchar2,
  SetValueFlags number := 3
) 
as begin pkg_xpobjecttype.Edit(pOID, pTypeName, pAssemblyName, SetValueFlags); end;
/
create or replace procedure %user%.p_xpobjecttype_d(
  pOID int
) 
as begin pkg_xpobjecttype.Del(pOID); end;
/