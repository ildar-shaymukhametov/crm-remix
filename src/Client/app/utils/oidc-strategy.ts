import type { StrategyVerifyCallback } from "remix-auth";
import type {
  OAuth2Profile,
  OAuth2StrategyVerifyParams,
} from "remix-auth-oauth2";
import { OAuth2Strategy } from "remix-auth-oauth2";

export interface OidcStrategyOptions {
  nonce: string;
  scope: string;
  authority: string;
  clientID: string;
  clientSecret: string;
  callbackURL: string;
}

export interface OidcExtraParams extends Record<string, string | number> {
  id_token: string;
  scope: string;
  expires_in: 86_400;
  token_type: "Bearer";
}

export interface OidcProfile extends OAuth2Profile {
  id: string;
  displayName: string;
  name: {
    familyName: string;
    givenName: string;
    middleName: string;
  };
  emails: Array<{ value: string }>;
  photos: Array<{ value: string }>;
  permissions: string[];
  extra: OidcExtraParams;
  _json: {
    sub: string;
    name: string;
    given_name: string;
    family_name: string;
    middle_name: string;
    nickname: string;
    preferred_username: string;
    profile: string;
    picture: string;
    website: string;
    email: string;
    email_verified: boolean;
    gender: string;
    birthdate: string;
    zoneinfo: string;
    locale: string;
    phone_number: string;
    phone_number_verified: boolean;
    address: {
      country: string;
    };
    updated_at: string;
  };
}

export class OidcStrategy<User> extends OAuth2Strategy<
  User,
  OidcProfile,
  OidcExtraParams
> {
  name = "oidc";
  private userInfoURL: string;
  private scope: string;
  private nonce: string;

  constructor(
    options: OidcStrategyOptions,
    verify: StrategyVerifyCallback<
      User,
      OAuth2StrategyVerifyParams<OidcProfile, OidcExtraParams>
    >
  ) {
    super(
      {
        authorizationURL: `${options.authority}/connect/authorize`,
        tokenURL: `${options.authority}/connect/token`,
        clientID: options.clientID,
        clientSecret: options.clientSecret,
        callbackURL: options.callbackURL,
      },
      verify
    );

    this.userInfoURL = `${options.authority}/connect/userinfo`;
    this.scope = options.scope || "openid profile email";
    this.nonce = options.nonce;
  }

  protected authorizationParams() {
    const urlSearchParams: Record<string, string> = {
      scope: this.scope,
    };

    if (this.nonce) {
      urlSearchParams.nonce = this.nonce;
    }

    return new URLSearchParams(urlSearchParams);
  }

  protected async userProfile(
    accessToken: string,
    params: OidcExtraParams
  ): Promise<OidcProfile> {
    let response = await fetch(this.userInfoURL, {
      headers: { Authorization: `Bearer ${accessToken}` },
    });

    let data: OidcProfile["_json"] = await response.json();

    let profile: OidcProfile = {
      provider: "oidc",
      displayName: data.name,
      id: data.sub,
      name: {
        familyName: data.family_name,
        givenName: data.given_name,
        middleName: data.middle_name,
      },
      emails: [{ value: data.email }],
      photos: [{ value: data.picture }],
      extra: params,
      permissions: [],
      _json: data,
    };

    return profile;
  }
}
