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
        view: "Company.Any.Other.View",
        update: "Company.Any.Other.Update"
      }
    },
    whereUserIsManager: {
      delete: "Company.WhereUserIsManager.Delete",
      other: {
        view: "Company.WhereUserIsManager.Other.View",
        update: "Company.WhereUserIsManager.Other.Update"
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
