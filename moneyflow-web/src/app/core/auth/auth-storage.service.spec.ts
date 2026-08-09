import { AuthResponse } from '../../features/auth/auth.models';
import { AuthStorageService } from './auth-storage.service';

describe('AuthStorageService', () => {
  let service: AuthStorageService;

  const createSession = (overrides: Partial<AuthResponse> = {}): AuthResponse => ({
    userId: 1,
    firstName: 'Carlos',
    lastName: 'Mansilla',
    email: 'carlos@example.com',
    accessToken: 'access-token',
    accessTokenExpiresAt: new Date(Date.now() + 5 * 60_000).toISOString(),
    refreshToken: 'refresh-token',
    refreshTokenExpiresAt: new Date(Date.now() + 24 * 60 * 60_000).toISOString(),
    ...overrides,
  });

  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    service = new AuthStorageService();
  });

  afterEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });

  it('stores a persistent session in localStorage', () => {
    const session = createSession();

    service.saveSession(session, true);

    expect(service.getSession()).toEqual(session);
    expect(localStorage.getItem('moneyflow_session')).not.toBeNull();
    expect(sessionStorage.getItem('moneyflow_session')).toBeNull();
    expect(service.isPersistentSession()).toBe(true);
  });

  it('stores a temporary session in sessionStorage', () => {
    const session = createSession();

    service.saveSession(session, false);

    expect(service.getSession()).toEqual(session);
    expect(localStorage.getItem('moneyflow_session')).toBeNull();
    expect(sessionStorage.getItem('moneyflow_session')).not.toBeNull();
    expect(service.isPersistentSession()).toBe(false);
  });

  it('keeps the user authenticated while the refresh token is valid', () => {
    service.saveSession(
      createSession({
        accessTokenExpiresAt: new Date(Date.now() - 60_000).toISOString(),
      }),
    );

    expect(service.isAccessTokenExpired()).toBe(true);
    expect(service.isRefreshTokenExpired()).toBe(false);
    expect(service.isAuthenticated()).toBe(true);
  });

  it('detects an access token that expires inside the safety margin', () => {
    service.saveSession(
      createSession({
        accessTokenExpiresAt: new Date(Date.now() + 20_000).toISOString(),
      }),
    );

    expect(service.isAccessTokenExpired()).toBe(false);
    expect(service.isAccessTokenExpired(30)).toBe(true);
  });

  it('removes malformed session data', () => {
    localStorage.setItem('moneyflow_session', '{invalid-json');

    expect(service.getSession()).toBeNull();
    expect(localStorage.getItem('moneyflow_session')).toBeNull();
  });

  it('clears both browser storages', () => {
    localStorage.setItem('moneyflow_session', '{}');
    sessionStorage.setItem('moneyflow_session', '{}');

    service.clearSession();

    expect(localStorage.getItem('moneyflow_session')).toBeNull();
    expect(sessionStorage.getItem('moneyflow_session')).toBeNull();
  });
});
