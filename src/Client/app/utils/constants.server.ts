export const claims = {
  company: {
    create: "Company.Create",
    any: {
      delete: "Company.Any.Delete",
      update: "Company.Any.Update",
      manager: {
        setFromNoneToSelf: "Company.Any.Manager.SetFromNoneToSelf",
        setFromAnyToNone: "Company.Any.Manager.SetFromAnyToNone",
        setFromNoneToAny: "Company.Any.Manager.SetFromNoneToAny",
        setFromAnyToAny: "Company.Any.Manager.SetFromAnyToAny",
        setFromAnyToSelf: "Company.Any.Manager.SetFromAnyToSelf",
        setFromSelfToAny: "Company.Any.Manager.SetFromSelfToAny",
        setFromSelfToNone: "Company.Any.Manager.SetFromSelfToNone"
      },
      other: {
        get: "Company.Any.Other.Get",
        set: "Company.Any.Other.Set"
      }
    },
    whereUserIsManager: {
      delete: "Company.WhereUserIsManager.Delete",
      other: {
        get: "Company.WhereUserIsManager.Other.Get",
        set: "Company.WhereUserIsManager.Other.Set"
      },
      manager: {
        setFromSelfToAny: "Company.WhereUserIsManager.Manager.SetFromSelfToAny",
        setFromSelfToNone:
          "Company.WhereUserIsManager.Manager.SetFromSelfToNone"
      }
    }
  }
};

// High level authorization data
export const permissions = {
  company: {
    create: "Company.Create",
    delete: "Company.Delete",
    update: "Company.Update",
    view: "Company.View"
  }
};
