import "@nice-digital/wdio-cucumber-steps/lib/given";
import { Given } from '@cucumber/cucumber';
import {cmsLogin} from "../support/action/cmsLogin";



//Uncomment below section to write custom step definitions

Given(/^I am logged in to Keystone CMS with username "([A-Z0-9_]+)" and password "([A-Z0-9_]+)"$/, cmsLogin);