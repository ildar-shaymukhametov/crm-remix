export const claims = {
  company: {
    create: "Company.Create",
    new: {
      setManagerToSelf: "Company.New.SetManagerToSelf",
      setManagerToAny: "Company.New.SetManagerToAny"
    },
    old: {
      any: {
        view: "Company.Old.Any.View",
        delete: "Company.Old.Any.Delete",
        update: "Company.Old.Any.Update",
        setManagerFromNoneToSelf: "Company.Old.Any.SetManagerFromNoneToSelf",
        setManagerFromAnyToNone: "Company.Old.Any.SetManagerFromAnyToNone",
        setManagerFromNoneToAny: "Company.Old.Any.SetManagerFromNoneToAny",
        setManagerFromAnyToAny: "Company.Old.Any.SetManagerFromAnyToAny",
        setManagerFromAnyToSelf: "Company.Old.Any.SetManagerFromAnyToSelf",
        setManagerFromSelfToAny: "Company.Old.Any.SetManagerFromSelfToAny",
        setManagerFromSelfToNone: "Company.Old.Any.SetManagerFromSelfToNone"
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
    view: "Company.View",
    setManager: "Company.SetManager"
  }
};
