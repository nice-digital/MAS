import "@nice-digital/wdio-cucumber-steps/lib/given";
import { Given } from '@cucumber/cucumber';
import {cmsLogin} from "../support/action/cmsLogin";



//Uncomment below section to write custom step definitions

Given(/^I am logged in to Keystone CMS with username and password$/, cmsLogin);