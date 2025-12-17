export function getRecaptchaToken(siteKey) {
  return grecaptcha.execute(siteKey, { action: "submit" });
}
