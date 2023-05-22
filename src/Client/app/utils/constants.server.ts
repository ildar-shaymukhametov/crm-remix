export const claims = {
  company: {
    create: "Company.Create",
    any: {
      view: "Company.Any.View",
      delete: "Company.Any.Delete",
      update: "Company.Any.Update",
      setManagerFromNoneToSelf: "Company.Any.SetManagerFromNoneToSelf",
      setManagerFromAnyToNone: "Company.Any.SetManagerFromAnyToNone",
      setManagerFromNoneToAny: "Company.Any.SetManagerFromNoneToAny",
      setManagerFromAnyToAny: "Company.Any.SetManagerFromAnyToAny",
      setManagerFromAnyToSelf: "Company.Any.SetManagerFromAnyToSelf",
      setManagerFromSelfToAny: "Company.Any.SetManagerFromSelfToAny",
      setManagerFromSelfToNone: "Company.Any.SetManagerFromSelfToNone",
    }
  }
};

// High level authorization data
export const permissions = {
  createCompany: "CreateCompany",
  deleteCompany: "DeleteCompany",
  updateCompany: "UpdateCompany",
  viewCompany: "ViewCompany"
};
