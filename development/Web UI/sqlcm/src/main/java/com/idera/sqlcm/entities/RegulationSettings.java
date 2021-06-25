package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

public class RegulationSettings {

    @JsonProperty("pci")
    private boolean pci;

    @JsonProperty("hipaa")
    private boolean hipaa;

    @JsonProperty("disa")
    private boolean disa;

    @JsonProperty("nerc")
    private boolean nerc;

	@JsonProperty("cis")
    private boolean cis;

    @JsonProperty("sox")
    private boolean sox;
    
    @JsonProperty("ferpa")
    private boolean ferpa;

    public boolean getPci() {
        return pci;
    }
    
    public void setPci(boolean pci) {
        this.pci = pci;
    }

    public boolean getHipaa() {
        return hipaa;
    }

    public void setHipaa(boolean hipaa) {
        this.hipaa = hipaa;
    }

    public boolean isDisa() {
		return disa;
	}

	public void setDisa(boolean disa) {
		this.disa = disa;
	}

	public boolean isNerc() {
		return nerc;
	}

	public void setNerc(boolean nerc) {
		this.nerc = nerc;
	}

	public boolean isCis() {
		return cis;
	}

	public void setCis(boolean cis) {
		this.cis = cis;
	}

	public boolean isSox() {
		return sox;
	}

	public void setSox(boolean sox) {
		this.sox = sox;
	}

	public boolean isFerpa() {
		return ferpa;
	}

	public void setFerpa(boolean ferpa) {
		this.ferpa = ferpa;
	}
}