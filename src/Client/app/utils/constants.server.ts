export const claims = {
  company: {
    create: "Company.Create",
    new: {
      setManagerToSelf: "Company.New.SetManagerToSelf",
      setManagerToNone: "Company.New.SetManagerToNone",
      setManagerToAny: "Company.New.SetManagerToAny",
    },
    any: {
      view: "Company.Any.View",
      delete: "Company.Any.Delete",
      update: "Company.Any.Update"
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
