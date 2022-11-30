export const permissions = {
  createCompany: "CreateCompany",
  deleteCompany: "DeleteCompany",
  updateCompany: "UpdateCompany",
  viewCompany: "ViewCompany"
};

export const routes = {
  companies: {
    index: "/companies",
    new: "/companies/new",
    view: (id: string | number | undefined) => `/companies/${id}`,
    edit: (id: string | number | undefined) => `/companies/${id}/edit`,
    delete: (id: string | number | undefined) => `/companies/${id}/delete`,
  },
  account: {
    access: "/account/access",
    profile: "/account/profile"
  }
};
